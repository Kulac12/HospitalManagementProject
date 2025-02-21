using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_three.Services.Abstract
{
    public interface IFirmOperationService
    {
        Task<IDataResponse<PaginationResponseModel<SubFirm>>> GetFirmDatatable(PaginationRequestModel model);

        #region Parent Firm | Firma grubu üzerindeki işlemleri temsil eden metotları barındırır.
        Task<IDataResponse<PaginationResponseModel<ParentFirmDataTableModel>>> GetParentFirmDatatable(PaginationRequestModel model);

        Task<IDataResponse<List<ParentFirmResponseModel>>> GetListParentFirm();
        Task<IResponse> DeleteParentFirm(Guid id);
        Task<IResponse> UpdateParentFirm(UpdateParentFirmRequestModel model);
        Task<IResponse> CreateParentFirm(CreateParentFirmRequestModel model);
        Task<IDataResponse<ParentFirmResponseModel>> GetParentFirm(Guid id);
        #endregion

        #region Firm | Firma tanımlama işlemlerini temsil eden metotları barındırır.
        Task<IResponse> CheckTaxNumberIxExists(string taxNumber);

        Task<IResponse> DeleteFirm(Guid id);
        Task<IDataResponse<List<GetFirmSelectListResponseModel>>> GetFirmSelectList(string search, bool? isOwnFirm = null);
        Task<IResponse> CreateFirm(CreateFirmRequestModel model);
        Task<IResponse> UpdateFirm(UpdateFirmInformationRequestModel model);
        Task<IDataResponse<GetFirmResponseModel>> GetFirm(Guid id);
        Task<IDataResponse<PaginationResponseModel<GetListFirmResponseModel>>> GetDatatableFirm(FirmTableReqModel model);
        #endregion

        #region Firm Contact | Firma iletişim bilgileri tanımlama alanıdır.
        Task<IResponse> CreateFirmContact(CreateFirmContactInformationRequestModel model);
        Task<IResponse> UpdateFirmContact(UpdateFirmContactInformationRequestModel model);
        Task<IResponse> DeleteFirmContact(Guid id);
        Task<IDataResponse<FirmContactResponseModel>> GetFirmContact(Guid id);
        Task<IDataResponse<PaginationResponseModel<DatatableListFirmContactResponseModel>>> GetDatatableListFirmContact(GetDatatableListFirmContactRequestModel model);
        #endregion

        #region Firm Location | Firma konum işlemleri alanıdır.
        Task<IDataResponse<List<GetFirmLocationResModel>>> GetFirmLocationSelectList(Guid firmId);
        Task<IDataResponse<PaginationResponseModel<DatatableListFirmLocationResponseModel>>> GetDatatableListFirmLocation(GetDatatableListFirmLocationRequestModel model);
        Task<IResponse> CreateFirmLocation(CreateFirmLocationInformationRequestModel model);
        Task<IResponse> UpdateFirmLocation(UpdateFirmLocationInformationRequestModel model);
        Task<IResponse> DeleteFirmLocation(Guid id);
        Task<IDataResponse<GetFirmLocationResModel>> GetFirmLocation(Guid id);

        #endregion

    }
