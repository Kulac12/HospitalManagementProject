using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_three.Services.Concretes
{
    public class FirmOperationService : IFirmOperationService
    {
        private readonly IMainProcessDbUnitOfWork mainProcessDbUnitOfWork;
        private readonly ILogService logService;
        private readonly IMapper mapper;
        private readonly IFileService fileService;

        public FirmOperationService(IMainProcessDbUnitOfWork mainProcessDbUnitOfWork,
            ILogService logService,
            IMapper mapper,
            IFileService fileService)
        {
            this.mainProcessDbUnitOfWork = mainProcessDbUnitOfWork;
            this.logService = logService;
            this.mapper = mapper;
            this.fileService = fileService;
        }


        #region New Firm List
        public async Task<IDataResponse<PaginationResponseModel<SubFirm>>> GetFirmDatatable(PaginationRequestModel model)
        {
            var data = await mainProcessDbUnitOfWork.FirmRepository
                .GetPaginateAllAsyncWithIncludeParams(model,
                x => x.ParentFirmId != null &&
                    (
                    x.Locations.Any(c => c.LocationName.Contains(model.QueryParam)) ||
                    x.Name.ToUpper().Contains(model.QueryParam.ToUpper()) ||
                    x.TaxNumber.Contains(model.QueryParam) ||
                    x.Name.ToLower().Contains(model.QueryParam.ToLower()) ||
                    x.FirmShortName.Contains(model.QueryParam) ||
                    x.FirmCode.Contains(model.QueryParam) ||
                    x.FirmShortName.Contains(model.QueryParam) ||
                    x.Name.ToLower().Contains(model.QueryParam.ToLower()) ||
                    x.Name.ToUpper().Contains(model.QueryParam.ToUpper())), false, x => x.ParentFirm, x => x.Locations.Where(c => !c.Deleted));

            var firmLocationTypes = await mainProcessDbUnitOfWork.LocationTypeRepository.GetAllAsync(null, false);
            var provinces = await mainProcessDbUnitOfWork.ProvinceRepository.GetAllAsync(null, false);
            var districts = await mainProcessDbUnitOfWork.DistrictRepository.GetAllAsync(null, false);
            var neighborhoods = await mainProcessDbUnitOfWork.NeighborhoodRepository.GetAllAsync(null, false);
            var streets = await mainProcessDbUnitOfWork.StreetRepository.GetAllAsync(null, false);
            var countries = await mainProcessDbUnitOfWork.CountryRepository.GetAllAsync(null, false);

            var resData = data.Data.Select(t => new SubFirm()
            {
                Id = t.Id,
                MainFirmId = t.ParentFirm?.Id,
                MainFirmName = t.ParentFirm?.Name,
                IsOwnMainFirm = t.ParentFirm?.IsOwnFirm,
                MainFirmShortName = t.ParentFirm?.FirmShortName,
                IsOwnFirm = t.IsOwnFirm,
                Name = t.Name,
                ShortName = t.FirmShortName,
                TaxNumber = t.TaxNumber,
                TaxAdministrationId = t.TaxAdministrationId,
                TaxAdministrationName = t.TaxAdministration?.Name,
                Logo = fileService.GetFileApiUrl(new GetFileApiUrlReqModel()
                {
                    EntityFileType = FileEntityNameModel.FirmLogo.ToString(),
                    EntityId = t.Id,
                    EntityName = FileEntityNameModel.FirmLogo.ToString()
                }).Result?.Data,
                LocationsCount = t.Locations?.Where(a => a.FirmId == t.Id).Count(),
                Locations = t.Locations?.Where(a => a.FirmId == t.Id).Select(k => new SubFirmLocation()
                {

                    Id = k.Id,
                    Code = k.Code,
                    CountryId = k.CountryId,
                    CountryName = countries.FirstOrDefault(t => t.Id == k.CountryId)?.Name,
                    DistrictId = k.DistrictId,
                    DistrictName = districts.FirstOrDefault(c => c.Id == k.DistrictId)?.Name,
                    LocationName = k.LocationName,
                    LocationTypeName = firmLocationTypes.FirstOrDefault(c => c.Id == k.LocationTypeId)?.Name,
                    ProvinceId = k.ProvinceId,
                    ProvinceName = provinces.FirstOrDefault(c => c.Id == k.ProvinceId)?.Name,
                    InvoiceTitle = k.InvoiceTitle,
                    IsInvoiceAddress = k.IsInvoiceAddress,
                    NeighboorhoodName = neighborhoods.FirstOrDefault(t => t.Id == k.NeighborhoodId)?.Name,
                    StreetName = streets.FirstOrDefault(t => t.Id == k.StreetId)?.Name,
                    Address = k.Address,
                    BuildingNumber = k.BuildingNumber,
                    DoorNumber = k.DoorNumber,
                    Floor = k.Floor,
                    PostalCode = k.PostalCode,
                    Xcoordinate = k.Xcoordinate,
                    Ycoordinate = k.Ycoordinate,
                    PostalAddress = k.PostalAddress
                }).ToList()
            }).ToList();

            return new SuccessDataResponse<PaginationResponseModel<SubFirm>>(new PaginationResponseModel<SubFirm>()
            {
                Data = resData,
                TotalCount = data.TotalCount
            });

        }

        #endregion


        #region Parent Firm | Firma grubu üzerindeki işlemleri temsil eden metotları barındırır.
        public async Task<IResponse> CreateParentFirm(CreateParentFirmRequestModel model)
        {
            await logService.CreateUserActionLog(new CreateUserActionLogModel()
            {
                ActionLogLevel = UserActionLogLevel.Information,
                ActionName = UserActionName.FirmOperations,
                Detail = $"{model.ParentFirmName} - Ana firma oluşturma isteği yapıldı.",
                RequestData = model
            }, true);
            var parentFirm = mapper.Map<Firm>(model);

            if (String.IsNullOrEmpty(parentFirm.FirmShortName.Trim()))
                parentFirm.FirmShortName = AlphabeticHelper.ConvertCharToEnglishAlphabeticalCharacter(parentFirm.Name.Substring(0, 3));
            else
                parentFirm.FirmShortName =
                    AlphabeticHelper.StringToUpperCase(AlphabeticHelper.ConvertCharToEnglishAlphabeticalCharacter
                    (parentFirm.FirmShortName));
            parentFirm.IsParentFirm = true;

            var firms = await mainProcessDbUnitOfWork.FirmRepository.GetAllDeletedControlAsync(x => x.IsParentFirm, true);
            var firmCodes = firms.OrderBy(f => f.CreateTime).Select(f => f.FirmCode);
            var lastFirmCode = firmCodes.LastOrDefault();
            parentFirm.FirmCode = lastFirmCode != null
                ? (Convert.ToInt64(lastFirmCode) + 1).ToString("0000")
                : "0001";

            await mainProcessDbUnitOfWork.FirmRepository.AddAsync(parentFirm);
            if (await mainProcessDbUnitOfWork.SaveChangesAsync() > 0)
                return new SuccessResponse(ResponseMessage.SuccessfullyAdded);
            return new ErrorResponse(ResponseMessage.ProcessFailed);
        }
        public async Task<IResponse> UpdateParentFirm(UpdateParentFirmRequestModel model)
        {
            await logService.CreateUserActionLog(new CreateUserActionLogModel()
            {
                ActionLogLevel = UserActionLogLevel.Information,
                ActionName = UserActionName.FirmOperations,
                Detail = $"{model.ParentFirmName} - Ana firma güncelleme isteği yapıldı."
            }, true);
            var firm = await mainProcessDbUnitOfWork.FirmRepository.GetAsync(c => c.Id == model.Id, false);
            if (firm == null)
                return new ErrorResponse(ResponseMessage.RecordNotFound);

            var parentFirm = mapper.Map(model, firm);

            if (String.IsNullOrEmpty(parentFirm.FirmShortName.Trim()))
                parentFirm.FirmShortName = AlphabeticHelper.ConvertCharToEnglishAlphabeticalCharacter(parentFirm.Name.Substring(0, 3));
            else
                parentFirm.FirmShortName =
                    AlphabeticHelper.StringToUpperCase(AlphabeticHelper.ConvertCharToEnglishAlphabeticalCharacter
                    (parentFirm.FirmShortName));

            mainProcessDbUnitOfWork.FirmRepository.Update(parentFirm);
            if (await mainProcessDbUnitOfWork.SaveChangesAsync() > 0)
                return new SuccessResponse(ResponseMessage.SuccessfullyUpdated);
            return new ErrorResponse(ResponseMessage.ProcessFailed);
        }
        public async Task<IResponse> DeleteParentFirm(Guid id)
        {

            var firm = await mainProcessDbUnitOfWork.FirmRepository.GetByIdAsync(id);
            if (firm == null)
                return new ErrorResponse(ResponseMessage.RecordNotFound);
            await logService.CreateUserActionLog(new CreateUserActionLogModel()
            {
                ActionLogLevel = UserActionLogLevel.Information,
                ActionName = UserActionName.FirmOperations,
                Detail = $"{firm.Name} - Ana firma silme isteği yapıldı."
            }, true);

            mainProcessDbUnitOfWork.FirmRepository.Delete(firm);
            var subFirms = await mainProcessDbUnitOfWork.FirmRepository.GetAllAsync(c => c.ParentFirmId == id);
            foreach (var item in subFirms)
            {
                item.ParentFirmId = null;
            }
            mainProcessDbUnitOfWork.FirmRepository.UpdateRange(subFirms);
            if (await mainProcessDbUnitOfWork.SaveChangesAsync() > 0)
                return new SuccessResponse(ResponseMessage.SuccessfullyDeleted);
            return new ErrorResponse(ResponseMessage.ProcessFailed);

        }
        public async Task<IDataResponse<ParentFirmResponseModel>> GetParentFirm(Guid id)
        {
            var firm = await mainProcessDbUnitOfWork.FirmRepository.GetByIdAsync(id);
            if (firm == null)
                return new ErrorDataResponse<ParentFirmResponseModel>(ResponseMessage.RecordNotFound);
            await logService.CreateUserActionLog(new CreateUserActionLogModel()
            {
                ActionLogLevel = UserActionLogLevel.Information,
                ActionName = UserActionName.FirmOperations,
                Detail = $"{firm.Name} - Ana firma detayı görüntülendi."
            }, true);
            var obj = mapper.Map<ParentFirmResponseModel>(firm);
            return new SuccessDataResponse<ParentFirmResponseModel>(obj);

        }
        public async Task<IDataResponse<List<ParentFirmResponseModel>>> GetListParentFirm()
        {
            await logService.CreateUserActionLog(new CreateUserActionLogModel()
            {
                ActionLogLevel = UserActionLogLevel.Information,
                ActionName = UserActionName.FirmOperations,
                Detail = $"Ana firma listesi görüntülendi."
            }, true);
            var parentFirmList = await mainProcessDbUnitOfWork.FirmRepository
                .GetAllAsync(x => x.ParentFirmId == null && x.IsParentFirm);
            var returnList = parentFirmList.Select(t => new ParentFirmResponseModel()
            {
                FirmInformation = t.Name,
                Id = t.Id
            }).ToList();
            return new SuccessDataResponse<List<ParentFirmResponseModel>>(returnList);
        }

        public async Task<IDataResponse<PaginationResponseModel<ParentFirmDataTableModel>>> GetParentFirmDatatable(PaginationRequestModel model)
        {
            var data = await mainProcessDbUnitOfWork.FirmRepository
                .GetPaginateAllAsyncWithIncludeParams(model, x => x.ParentFirmId == null && (x.Name.Contains(model.QueryParam) || x.FirmShortName.Contains(model.QueryParam)), false);
            var resData = data.Data.Select(c => new ParentFirmDataTableModel()
            {
                Id = c.Id,
                Name = c.Name,
                ParentFirmShortName = c.FirmShortName
            }).ToList();

            return new SuccessDataResponse<PaginationResponseModel<ParentFirmDataTableModel>>(new PaginationResponseModel<ParentFirmDataTableModel>()
            {
                Data = resData,
                TotalCount = data.TotalCount
            });

        }

        #endregion

        #region Firm | Firma tanımlama işlemlerini temsil eden metotları barındırır.

        #region Private Methods

        private async Task<bool> UploadFile(Guid entityId, IFormFile formFile, FileEntityNameModel fileEntityNameModel)
        {
            try
            {
                if (formFile == null)
                {
                    await fileService.DeleteFileEntity(entityId, fileEntityNameModel.ToString(), fileEntityNameModel.ToString());
                    return true;
                }
                else
                {
                    var fileSaveResult = await fileService.UploadFileSingle(formFile,
                                 fileEntityNameModel.ToString(),
                                 fileEntityNameModel.ToString(),
                                 entityId);
                    if (fileSaveResult.Success)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }


        }

        /// <summary>
        /// Firma kodu oluşturmaya yarayan metottur.
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="districtId"></param>
        /// <returns></returns>
        async Task<string> FirmCodeGenerator(Guid? countryId, Guid provinceId, Guid districtId, Guid? parentFirmId)
        {
            //var country = await mainProcessDbUnitOfWork.CountryRepository.GetAsync(x => x.Id == countryId);
            var province = await mainProcessDbUnitOfWork.ProvinceRepository.GetAsync(x => x.Id == provinceId, false);
            var region = await mainProcessDbUnitOfWork.RegionRepository.GetAsync(x => x.Id == province.RegionId, false);
            var district = await mainProcessDbUnitOfWork.DistrictRepository.GetAsync(x => x.Id == districtId, false);
            var country = await mainProcessDbUnitOfWork.CountryRepository.GetAsync(x => x.Id == countryId, false);

            string code = "";
            code += country?.CountryShortCode + "-";
            code += region.Code + "-";
            code += province.Code + "-";
            code += district.Code + "-";
            var firmGroupList = await mainProcessDbUnitOfWork.FirmRepository.GetAllAsync(x => x.FirmCode.Contains(code));
            if (firmGroupList.Count > 0)
            {
                var newLocationCode = (firmGroupList.Count + 1).ToString();
                newLocationCode = Calculator.GroupCodeCountCalculator(newLocationCode, 2);
                code += newLocationCode;
            }
            else
                code += "01";

            var parentFirm = parentFirmId != null
                ? await mainProcessDbUnitOfWork.FirmRepository.GetByIdAsync((Guid)parentFirmId)
                : null;

            return parentFirm != null ?
                string.Concat(parentFirm.FirmCode, "-", code)
                : code;

        }

        /// <summary>
        /// Firma lokasyon kodu oluşturmaya yarayan metottur.
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="districtId"></param>
        /// <param name="neighborhoodId"></param>
        /// <param name="streetId"></param>
        /// <param name="locationTypeId"></param>
        /// <returns></returns>
        async Task<string> FirmLocationCodeGenerator(Guid? countryId, Guid provinceId, Guid districtId, Guid? neighborhoodId, Guid? streetId, Guid? locationTypeId)
        {
            var province = await mainProcessDbUnitOfWork.ProvinceRepository.GetAsync(x => x.Id == provinceId, false);
            var region = await mainProcessDbUnitOfWork.RegionRepository.GetAsync(x => x.Id == province.RegionId, false);
            var district = await mainProcessDbUnitOfWork.DistrictRepository.GetAsync(x => x.Id == districtId, false);
            var country = await mainProcessDbUnitOfWork.CountryRepository.GetAsync(c => c.Id == countryId, false);
            var locationType = await mainProcessDbUnitOfWork.LocationTypeRepository.GetAsync(c => c.Id == locationTypeId, false);
            string code = "";

            code += country?.CountryShortCode + "-" ?? "";
            code += region?.Code + "-" ?? "";
            code += province?.Code + "-" ?? "";
            code += district?.Code + "-" ?? "";

            string neighborhoodIdString = neighborhoodId?.ToString() ?? "";
            for (int i = neighborhoodIdString.Length; i < 5; i++)
                neighborhoodIdString = "0" + neighborhoodIdString;

            code += neighborhoodIdString + "-";
            string streetIdString = "";
            if (streetId == null)
                streetIdString = "000000";
            else
                streetIdString = streetId.ToString();

            for (int i = streetIdString.Length; i < 6; i++)
                streetIdString = "0" + streetIdString;

            code += streetIdString;
            code += "-";

            for (int i = locationType.Code.Length; i < 2; i++)
                locationType.Code = "0" + locationType.Code;

            code += locationType.Code;
            code += "-";
            var firmGroupLocationList = await mainProcessDbUnitOfWork.FirmLocationRepository.GetAllAsync(x => x.Code.Contains(code));
            if (firmGroupLocationList.Count > 0)
            {
                var newLocationCode = (firmGroupLocationList.Count + 1).ToString();
                newLocationCode = Calculator.GroupCodeCountCalculator(newLocationCode, 2);
                //02-0001-35023481-01-01,  02-0001-35023481-01-02
                code += newLocationCode;
            }
            else
                code += "01";

            return code;

        }
        #endregion

        public async Task<IResponse> CheckTaxNumberIxExists(string taxNumber)
        {
            var firm = await mainProcessDbUnitOfWork.FirmRepository
                .GetAsync(c => c.TaxNumber == taxNumber, false);

            if (firm != null)
                return new ErrorResponse(ResponseMessage.FirmTaxNumberIsExists);
            return new SuccessResponse();
        }

        /// <summary>
        /// Firma oluşturmaya yarayan metottur. Kendisi ve alt içeriklerini oluşturmaya yarar.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IResponse> CreateFirm(CreateFirmRequestModel model)
        {
            if (model.FirmInformation.ParentFirmId == null)
                return new ErrorResponse("Ana firma bilgisi zorunludur.");

            if (string.IsNullOrEmpty(model.FirmInformation.TaxNumber))
                return new ErrorResponse("Vergi No/TC Bilgisi zorunludur.");

            var isFirmExists = await mainProcessDbUnitOfWork.FirmRepository.GetAsync(c => c.TaxNumber == model.FirmInformation.TaxNumber, false);
            if (isFirmExists != null)
                return new ErrorResponse("Bu Vergi No/TC bilgisi daha önce eklenmiş.");

            var locationTypes = await mainProcessDbUnitOfWork.LocationTypeRepository
                .GetAllAsync(x => model.FirmLocations.Select(x => x.LocationTypeId).Contains(x.Id), false);

            if (!locationTypes.Any(b => b.IsDefaultLocation))
                return new ErrorResponse(ResponseMessage.YouHaveNotDefinedAnAddressInTheMainOfficeTypeYouNeedToDefine);

            //var firmContactsLocationsDummyIds = model.FirmContacts.Select(b => b.FirmLocationDummyId).ToList();
            //var firmLocationsByDummyIds = model.FirmLocations.Where(x => firmContactsLocationsDummyIds.Contains(x.FirmLocationDummyId));

            //var locationTypesByFirmContacts = await mainProcessDbUnitOfWork.LocationTypeRepository
            //   .GetAllAsync(x => firmLocationsByDummyIds.Select(x => x.LocationTypeId).Contains(x.Id), false);

            //if (!locationTypesByFirmContacts.Any(b => b.IsDefaultLocation))
            //    return new ErrorResponse(ResponseMessage.YouHaveNotDefinedAMainLocationContactInformation);


            var firm = mapper.Map<Firm>(model.FirmInformation);



            var firmLocations = mapper.Map<List<FirmLocation>>(model.FirmLocations);

            #region<---------------------- Firma ekleme işlemi --------------------->
            var mainLocationType = await mainProcessDbUnitOfWork.LocationTypeRepository.GetAsync(x => x.IsDefaultLocation, false);
            if (mainLocationType == null)
                return new ErrorResponse(ResponseMessage.ProcessFailed);
            var mainFirmLocation = model.FirmLocations.FirstOrDefault(x => x.LocationTypeId == mainLocationType.Id);
            firm.IsParentFirm = false;
            firm.FirmCode = await FirmCodeGenerator(mainFirmLocation.CountryId, mainFirmLocation.ProvinceId, mainFirmLocation.DistrictId, firm.ParentFirmId);
            firm.Name = firm.Name.Trim();
            if (String.IsNullOrEmpty(firm.FirmShortName))
                firm.FirmShortName = AlphabeticHelper.ConvertCharToEnglishAlphabeticalCharacter(firm.Name.Substring(0, 3));
            else
                firm.FirmShortName =
                    AlphabeticHelper.ConvertCharToEnglishAlphabeticalCharacter
                    (firm.FirmShortName.Trim());

            await mainProcessDbUnitOfWork.FirmRepository.AddAsync(firm);
            #endregion

            #region <---------------------- Firma Sektörü Ekleme işlemi --------------------->
            List<FirmSectorType> firmSectorTypes = new();
            foreach (var item in model.FirmInformation.SectorTypeIds)
            {
                firmSectorTypes.Add(new FirmSectorType()
                {
                    FirmId = firm.Id,
                    SectorTypeId = item
                });
            }
            await mainProcessDbUnitOfWork.FirmSectorTypeRepository.AddRangeAsync(firmSectorTypes);
            #endregion

            #region <---------------------- Firma lokasyon ekleme işlemi --------------------->

            //firmLocations.ForEach(x => x.Code = FirmLocationCodeGenerator(x.CountryId, x.ProvinceId, x.DistrictId, x.NeighborhoodId, x.StreetId, x.LocationTypeId).Result);
            firmLocations.ForEach(x => x.FirmId = firm.Id);
            firmLocations.ForEach(x => x.LocationName = x.LocationName);
            await mainProcessDbUnitOfWork.FirmLocationRepository.AddRangeAsync(firmLocations);

            #endregion

            #region  <---------------------- Firma İletişim Bilgisi Ekleme İşlemi --------------------->
            if (model.FirmContacts != null)
            {
                foreach (var item in model.FirmContacts)
                {
                    var firmLocation = firmLocations.FirstOrDefault(x => x.FirmLocationDummyId == item.FirmLocationDummyId);
                    item.FirmLocationId = firmLocation.Id;
                    item.FirstName = item.FirstName;
                    item.LastName = item.LastName;
                    item.Email = item.Email.ToLower();
                }
                var firmContacts = mapper.Map<List<FirmContact>>(model.FirmContacts);
                firmContacts.ForEach(x => x.FirmId = firm.Id);
                await mainProcessDbUnitOfWork.FirmContactRepository.AddRangeAsync(firmContacts);
            }


            #endregion

            #region <---------------------- Firma Muhasebesel Verilerinin Eklenmesi İşlemi --------------------->
            //var invoiceInformation = new FirmInvoiceInformation()
            //{
            //    CurrentInvoiceName = firm.Name,
            //    FirmCode = firm.FirmCode,
            //    CurrentProvinceId = mainFirmLocation?.ProvinceId,
            //    CurrentDistrictId = mainFirmLocation?.DistrictId,
            //    CurrentAddress = mainFirmLocation.Address,
            //    TaxAdministrationId = firm.TaxAdministrationId,
            //    TaxNumber = firm.TaxNumber,
            //    FirmId = firm.Id,

            //};
            //await mainProcessDbUnitOfWork.FirmInvoiceInformationRepository.AddAsync(invoiceInformation);
            #endregion

            var processRecordStatus = await mainProcessDbUnitOfWork.SaveChangesAsync();

            #region <------------- Firma Logo İşlemleri -------------->
            var fileSaveResult = await UploadFile(firm.Id, model.FirmInformation.Logo, FileEntityNameModel.FirmLogo);
            #endregion


            if (processRecordStatus > 0 && fileSaveResult)
                return new SuccessResponse(ResponseMessage.SuccessfullyAdded);
            if (processRecordStatus > 0 && !fileSaveResult)
                return new SuccessResponse(ResponseMessage.DataSuccessfullyAddedButFileNotSaved);
            return new ErrorResponse(ResponseMessage.ProcessFailed);
        }

        public async Task<IResponse> UpdateFirm(UpdateFirmInformationRequestModel model)
        {
            var firm = await mainProcessDbUnitOfWork.FirmRepository.GetByIdAsync(model.Id);
            if (firm == null)
                return new ErrorResponse(ResponseMessage.FirmNotFound);

            if (string.IsNullOrEmpty(model.TaxNumber))
                return new ErrorResponse("Vergi No/TC Bilgisi zorunludur.");

            var isFirmExists = await mainProcessDbUnitOfWork.FirmRepository.GetAsync(c => c.Id != model.Id && c.TaxNumber == model.TaxNumber, false);
            if (isFirmExists != null)
                return new ErrorResponse("Bu Vergi No/TC bilgisi daha önce eklenmiş.");

            var firmUpdateModel = mapper.Map(model, firm);
            firmUpdateModel.Name = firmUpdateModel.Name;
            firmUpdateModel.FirmShortName = firmUpdateModel.FirmShortName;
            mainProcessDbUnitOfWork.FirmRepository.Update(firmUpdateModel);


            var oldFirmSectors = await mainProcessDbUnitOfWork.FirmSectorTypeRepository
                .GetAllAsync(x => x.FirmId == model.Id, false);

            mainProcessDbUnitOfWork.FirmSectorTypeRepository.RemoveRange(oldFirmSectors);
            #region <---------------------- Firma Sektörü Ekleme işlemi --------------------->
            List<FirmSectorType> firmSectorTypes = new();
            foreach (var item in model.SectorTypeIds)
            {
                firmSectorTypes.Add(new FirmSectorType()
                {
                    FirmId = firm.Id,
                    SectorTypeId = item
                });
            }
            await mainProcessDbUnitOfWork.FirmSectorTypeRepository.AddRangeAsync(firmSectorTypes);
            #endregion
            var firmSaveStatus = await mainProcessDbUnitOfWork.SaveChangesAsync();

            var fileSaveResult = await UploadFile(firm.Id, model.Logo, FileEntityNameModel.FirmLogo);

            if (firmSaveStatus > 0 && fileSaveResult)
                return new SuccessResponse(ResponseMessage.FirmInformationSuccessfullyUpdated);
            if (firmSaveStatus > 0 && !fileSaveResult)
                return new SuccessResponse(ResponseMessage.DataSuccessfullyUpdatedButFileNotSaved);

            return new ErrorResponse(ResponseMessage.ProcessFailed);

        }

        public async Task<IDataResponse<PaginationResponseModel<GetListFirmResponseModel>>> GetDatatableFirm(FirmTableReqModel model)
        {
            var firmData = await mainProcessDbUnitOfWork.FirmRepository
                .GetPaginateAllAsyncWithIncludeParams(model.PaginationRequestModel,
                x =>
                (model.IsParentFirm != null ? model.IsParentFirm == true ? x.IsParentFirm : !x.IsParentFirm : true) &&
                (x.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                x.TaxNumber.Contains(model.PaginationRequestModel.QueryParam) ||
                x.ParentFirm.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                x.FirmCode.Contains(model.PaginationRequestModel.QueryParam) ||
                x.TaxNumber.Contains(model.PaginationRequestModel.QueryParam) ||
                x.TaxAdministration.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                x.ParentFirm.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                x.FirmCode.Contains(model.PaginationRequestModel.QueryParam) ||
                x.SectorType.Name.Contains(model.PaginationRequestModel.QueryParam)),
                true, x => x.ParentFirm, x => x.SectorType, x => x.TaxAdministration, x => x.Locations.Where(c => !c.Deleted));

            var mappedList = mapper.Map<List<GetListFirmResponseModel>>(firmData.Data);
            mappedList.ForEach(x =>
            {
                x.Logo = fileService.GetFileApiUrl(new GetFileApiUrlReqModel()
                {
                    EntityFileType = FileEntityNameModel.FirmLogo.ToString(),
                    EntityId = x.Id,
                    EntityName = FileEntityNameModel.FirmLogo.ToString()

                }).Result.Data;

            });
            if (model.PaginationRequestModel.OrderColumnName is null)
                mappedList = mappedList.OrderByDescending(c => c.IsParentFirm).ThenBy(c => c.ParentFirmId).ToList();
            return new SuccessDataResponse<PaginationResponseModel<GetListFirmResponseModel>>(new PaginationResponseModel<GetListFirmResponseModel>()
            {
                Data = mappedList,
                TotalCount = firmData.TotalCount
            });
        }

        public async Task<IDataResponse<GetFirmResponseModel>> GetFirm(Guid id)
        {
            var firm = await mainProcessDbUnitOfWork.FirmRepository.GetByIdAsync(id);
            if (firm == null)
                return new ErrorDataResponse<GetFirmResponseModel>(ResponseMessage.FirmNotFound);
            var mappedFirm = mapper.Map<GetFirmResponseModel>(firm);
            var sectorTypes = await mainProcessDbUnitOfWork.FirmSectorTypeRepository
                .GetAllAsync(c => c.FirmId == id, false);
            mappedFirm.SectorTypeIds = sectorTypes.Select(t => t.SectorTypeId).ToList();
            var firmLogoApiUrl = await fileService.GetFileApiUrl(new GetFileApiUrlReqModel()
            {
                EntityFileType = FileEntityNameModel.FirmLogo.ToString(),
                EntityId = id,
                EntityName = FileEntityNameModel.FirmLogo.ToString()
            });
            if (firmLogoApiUrl.Success)
                mappedFirm.Logo = firmLogoApiUrl.Data;
            return new SuccessDataResponse<GetFirmResponseModel>(mappedFirm);
        }

        public async Task<IDataResponse<List<GetFirmSelectListResponseModel>>> GetFirmSelectList(string search, bool? isOwnFirm = null)
        {
            var firmList = await mainProcessDbUnitOfWork.FirmRepository.GetAllAsync(x => x.ParentFirmId != null && x.Name.Contains(search) && (isOwnFirm == null || x.IsOwnFirm == isOwnFirm));
            var mappedList = mapper.Map<List<GetFirmSelectListResponseModel>>(firmList);
            mappedList.ForEach(x =>
            {
                x.Logo = fileService.GetFileApiUrl(new GetFileApiUrlReqModel()
                {
                    EntityFileType = FileEntityNameModel.FirmLogo.ToString(),
                    EntityId = x.Id,
                    EntityName = FileEntityNameModel.FirmLogo.ToString()

                }).Result.Data;
            });
            return new SuccessDataResponse<List<GetFirmSelectListResponseModel>>(mappedList);
        }

        public async Task<IResponse> DeleteFirm(Guid id)
        {
            var firm = await mainProcessDbUnitOfWork.FirmRepository.GetByIdAsync(id);
            if (firm == null)
                return new ErrorDataResponse<GetFirmResponseModel>(ResponseMessage.FirmNotFound);

            mainProcessDbUnitOfWork.FirmRepository.Delete(firm);
            if (await mainProcessDbUnitOfWork.SaveChangesAsync() > 0)
                return new SuccessResponse(ResponseMessage.SuccessfullyDeleted);
            return new ErrorResponse(ResponseMessage.ProcessFailed);
        }

        #endregion

        #region Firm Contact | Firma iletişim bilgileri tanımlama alanıdır.
        public async Task<IResponse> CreateFirmContact(CreateFirmContactInformationRequestModel model)
        {
            var firmContact = mapper.Map<FirmContact>(model);
            firmContact.FirstName = firmContact.FirstName;
            firmContact.LastName = firmContact.LastName;
            firmContact.Email = firmContact.Email.ToLower();
            await mainProcessDbUnitOfWork.FirmContactRepository.AddAsync(firmContact);
            if (await mainProcessDbUnitOfWork.SaveChangesAsync() > 0)
                return new SuccessResponse(ResponseMessage.SuccessfullyAdded);
            return new ErrorResponse(ResponseMessage.ProcessFailed);
        }
        public async Task<IResponse> UpdateFirmContact(UpdateFirmContactInformationRequestModel model)
        {
            var firmContactIsExists = await mainProcessDbUnitOfWork.FirmContactRepository.GetByIdAsync(model.Id);
            if (firmContactIsExists == null)
                return new ErrorResponse(ResponseMessage.RecordNotFound);
            var firmContact = mapper.Map(model, firmContactIsExists);
            firmContact.FirstName = firmContact.FirstName;
            firmContact.LastName = firmContact.LastName;
            firmContact.Email = firmContact.Email.ToLower();
            mainProcessDbUnitOfWork.FirmContactRepository.Update(firmContact);
            if (await mainProcessDbUnitOfWork.SaveChangesAsync() > 0)
                return new SuccessResponse(ResponseMessage.SuccessfullyUpdated);
            return new ErrorResponse(ResponseMessage.ProcessFailed);
        }
        public async Task<IResponse> DeleteFirmContact(Guid id)
        {
            var firmContactIsExists = await mainProcessDbUnitOfWork.FirmContactRepository.GetByIdAsync(id);
            if (firmContactIsExists == null)
                return new ErrorResponse(ResponseMessage.RecordNotFound);
            mainProcessDbUnitOfWork.FirmContactRepository.Delete(firmContactIsExists);
            if (await mainProcessDbUnitOfWork.SaveChangesAsync() > 0)
                return new SuccessResponse(ResponseMessage.SuccessfullyDeleted);
            return new ErrorResponse(ResponseMessage.ProcessFailed);
        }
        public async Task<IDataResponse<FirmContactResponseModel>> GetFirmContact(Guid id)
        {
            var firmContactIsExists = await mainProcessDbUnitOfWork.FirmContactRepository.GetByIdAsync(id);
            if (firmContactIsExists == null)
                return new ErrorDataResponse<FirmContactResponseModel>(ResponseMessage.RecordNotFound);
            var returnObj = mapper.Map<FirmContactResponseModel>(firmContactIsExists);
            return new SuccessDataResponse<FirmContactResponseModel>(returnObj);
        }
        public async Task<IDataResponse<PaginationResponseModel<DatatableListFirmContactResponseModel>>> GetDatatableListFirmContact(GetDatatableListFirmContactRequestModel model)
        {
            var tableResponse = await mainProcessDbUnitOfWork.FirmContactRepository
               .GetPaginateAllAsyncWithIncludeParams(model.PaginationRequestModel,
                       s => s.FirmId == model.FirmId &&
                       (s.CommunicationLanguage.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                       s.FirstName.Contains(model.PaginationRequestModel.QueryParam) ||
                       s.LastName.Contains(model.PaginationRequestModel.QueryParam) ||
                       s.Email.Contains(model.PaginationRequestModel.QueryParam) ||
                       s.OccupationTitle.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                       s.Phone.Contains(model.PaginationRequestModel.QueryParam) ||
                       s.MobilePhone.Contains(model.PaginationRequestModel.QueryParam) ||
                       s.InternalNumber.Contains(model.PaginationRequestModel.QueryParam) ||
                       s.FirmLocation.LocationName.Contains(model.PaginationRequestModel.QueryParam)),
                       true,
                       x => x.Firm, x => x.OccupationTitle, x => x.FirmLocation, x => x.CommunicationLanguage);

            var responseModel = mapper.Map<List<DatatableListFirmContactResponseModel>>(tableResponse.Data);
            var users = await mainProcessDbUnitOfWork.UserRepository.GetAllAsync();
            responseModel.ForEach(x =>
            {
                var user = users.FirstOrDefault(a => a.FirmContactId == x.Id);
                if (user != null)
                {
                    x.IsHaveUser = true;
                    x.UserId = user.Id;
                }
                else
                {
                    x.IsHaveUser = false;
                    x.UserId = null;
                }
            });

            return new SuccessDataResponse<PaginationResponseModel<DatatableListFirmContactResponseModel>>(new PaginationResponseModel<DatatableListFirmContactResponseModel>()
            {
                Data = responseModel,
                TotalCount = tableResponse.TotalCount
            });
        }

        #endregion

        #region Firm Location | Firma konum işlemleri alanıdır.

        private string CreateAddress(
            string neighboorhoodName,
            string streetName,
            string doorNumber,
            string floor,
            string buildingNumber,
            string postalCode,
            string districtName,
            string provinceName,
            string countryName
            )
        {
            string address = string.Empty;
            if (!String.IsNullOrEmpty(neighboorhoodName))
            {
                address += neighboorhoodName + " Mahallesi ";
            }
            if (!String.IsNullOrEmpty(streetName))
            {
                address += streetName + " Caddesi ";
            }
            if (!String.IsNullOrEmpty(buildingNumber))
            {
                address += "Bina No: " + buildingNumber + " ";
            }
            if (!String.IsNullOrEmpty(floor))
            {
                address += "Kat: " + floor + " ";
            }
            if (!String.IsNullOrEmpty(doorNumber))
            {
                address += "Kapı No: " + doorNumber + " ";
            }
            if (!String.IsNullOrEmpty(postalCode))
            {
                address += "Posta Kodu: " + postalCode + " ";
            }
            if (!String.IsNullOrEmpty(districtName))
            {
                address += districtName;
            }
            if (!String.IsNullOrEmpty(provinceName))
            {
                address += "/" + provinceName;
            }
            if (!String.IsNullOrEmpty(countryName))
            {
                address += "/" + countryName;
            }

            return address;
        }
        public async Task<IResponse> CreateFirmLocation(CreateFirmLocationInformationRequestModel model)
        {
            var createLocationObj = mapper.Map<FirmLocation>(model);

            var neighborhood = await mainProcessDbUnitOfWork.NeighborhoodRepository
                .GetAsync(c => c.Id == model.NeighborhoodId, false);
            var street = await mainProcessDbUnitOfWork.StreetRepository
                .GetAsync(c => c.Id == model.StreetId, false);
            var district = await mainProcessDbUnitOfWork.DistrictRepository
                .GetAsyncWithIncludeParams(c => c.Id == model.DistrictId, false, c => c.Province.Country);

            createLocationObj.Address =
                CreateAddress(
                    neighborhood?.Name,
                    street?.Name,
                    model.DoorNumber,
                    model.Floor,
                    model.BuildingNumber,
                    model.PostalCode,
                    district?.Name,
                    district?.Province?.Name,
                    district?.Province?.Country?.Name);

            createLocationObj.DoorNumber = createLocationObj.DoorNumber;
            createLocationObj.BuildingNumber = createLocationObj.BuildingNumber;
            createLocationObj.Floor = createLocationObj.Floor;
            createLocationObj.LocationName = createLocationObj.LocationName;
            await mainProcessDbUnitOfWork.FirmLocationRepository.AddAsync(createLocationObj);
            if (await mainProcessDbUnitOfWork.SaveChangesAsync() > 0)
                return new SuccessResponse(ResponseMessage.SuccessfullyAdded);
            return new ErrorResponse(ResponseMessage.ProcessFailed);
        }
        public async Task<IResponse> UpdateFirmLocation(UpdateFirmLocationInformationRequestModel model)
        {
            var firmLocationIsExists = await mainProcessDbUnitOfWork.FirmLocationRepository.GetByIdAsync(model.Id);
            if (firmLocationIsExists == null)
                return new ErrorResponse(ResponseMessage.RecordNotFound);
            var firmLocation = mapper.Map(model, firmLocationIsExists);
            var neighborhood = await mainProcessDbUnitOfWork.NeighborhoodRepository
                .GetAsync(c => c.Id == model.NeighborhoodId, false);
            var street = await mainProcessDbUnitOfWork.StreetRepository
                .GetAsync(c => c.Id == model.StreetId, false);
            var district = await mainProcessDbUnitOfWork.DistrictRepository
                .GetAsyncWithIncludeParams(c => c.Id == model.DistrictId, false, c => c.Province.Country);

            firmLocation.Address =
                CreateAddress(
                    neighborhood?.Name,
                    street?.Name,
                    model.DoorNumber,
                    model.Floor,
                    model.BuildingNumber,
                    model.PostalCode,
                    district?.Name,
                    district?.Province?.Name,
                    district?.Province?.Country?.Name);
            firmLocation.DoorNumber = firmLocation.DoorNumber;
            firmLocation.BuildingNumber = firmLocation.BuildingNumber;
            firmLocation.Floor = firmLocation.Floor;
            firmLocation.LocationName = firmLocation.LocationName;
            if (firmLocation.IsInvoiceAddress != null && !(bool)firmLocation.IsInvoiceAddress)
            {
                firmLocation.InvoiceTitle = null;
            }
            mainProcessDbUnitOfWork.FirmLocationRepository.Update(firmLocation);
            if (await mainProcessDbUnitOfWork.SaveChangesAsync() > 0)
                return new SuccessResponse(ResponseMessage.SuccessfullyUpdated);
            return new ErrorResponse(ResponseMessage.ProcessFailed);
        }
        public async Task<IResponse> DeleteFirmLocation(Guid id)
        {
            var firmLocation = await mainProcessDbUnitOfWork.FirmLocationRepository.GetByIdAsync(id);
            if (firmLocation == null)
                return new ErrorResponse(ResponseMessage.RecordNotFound);

            mainProcessDbUnitOfWork.FirmLocationRepository.Delete(firmLocation);
            if (await mainProcessDbUnitOfWork.SaveChangesAsync() > 0)
                return new SuccessResponse(ResponseMessage.SuccessfullyUpdated);
            return new ErrorResponse(ResponseMessage.ProcessFailed);


        }
        public async Task<IDataResponse<GetFirmLocationResModel>> GetFirmLocation(Guid id)
        {
            var firmLocation = await mainProcessDbUnitOfWork.FirmLocationRepository.GetByIdAsync(id);
            if (firmLocation == null)
                return new ErrorDataResponse<GetFirmLocationResModel>(ResponseMessage.RecordNotFound);
            var returnObj = mapper.Map<GetFirmLocationResModel>(firmLocation);
            return new SuccessDataResponse<GetFirmLocationResModel>(returnObj);
        }

        public async Task<IDataResponse<PaginationResponseModel<DatatableListFirmLocationResponseModel>>> GetDatatableListFirmLocation(GetDatatableListFirmLocationRequestModel model)
        {
            var tableResponse = await mainProcessDbUnitOfWork.FirmLocationRepository
               .GetPaginateAllAsyncWithIncludeParams(model.PaginationRequestModel,
                       s => s.FirmId == model.FirmId &&
                       (s.Address.Contains(model.PaginationRequestModel.QueryParam) ||
                       s.Code.Contains(model.PaginationRequestModel.QueryParam) ||
                        s.BuildingNumber.Contains(model.PaginationRequestModel.QueryParam) ||
                        s.DoorNumber.Contains(model.PaginationRequestModel.QueryParam) ||
                        s.Country.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                        s.District.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                        s.Province.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                        s.Street.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                        s.Neighborhood.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                        s.LocationName.Contains(model.PaginationRequestModel.QueryParam) ||
                        s.LocationType.Name.Contains(model.PaginationRequestModel.QueryParam) ||
                        s.PostalCode.Contains(model.PaginationRequestModel.QueryParam)),
                      false, x => x.Firm, x => x.District, x => x.Province, x => x.Province.Country, x => x.LocationType,
                       x => x.Neighborhood, x => x.Street);

            var responseModel = mapper.Map<List<DatatableListFirmLocationResponseModel>>(tableResponse.Data);

            return new SuccessDataResponse<PaginationResponseModel<DatatableListFirmLocationResponseModel>>(new PaginationResponseModel<DatatableListFirmLocationResponseModel>()
            {
                Data = responseModel,
                TotalCount = tableResponse.TotalCount
            });



        }
        public async Task<IDataResponse<List<GetFirmLocationResModel>>> GetFirmLocationSelectList(Guid firmId)
        {
            var firmLocations = await mainProcessDbUnitOfWork.FirmLocationRepository.GetAllAsync(x => x.FirmId == firmId);
            var mappedList = mapper.Map<List<GetFirmLocationResModel>>(firmLocations);
            return new SuccessDataResponse<List<GetFirmLocationResModel>>(mappedList);
        }

        #endregion

    }
}
