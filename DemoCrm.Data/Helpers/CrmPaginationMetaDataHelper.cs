using DemoCrm.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoCrm.Data.Helpers
{
    public class CrmPaginationMetaDataHelper<T>
    {
        public static string GetVendorSpecificPaginationMetaData(PagedList<T> entityPagedList)
        {
            var paginationMetadata = new
            {
                totalCount = entityPagedList.TotalCount,
                pageSize = entityPagedList.PageSize,
                currentPage = entityPagedList.CurrentPage,
                totalPages = entityPagedList.TotalPages,
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata);
        }

        public static string GetNonVendorPaginationMetaData(PagedList<T> entityPagedList, 
            IUrlHelper urlHelper, 
            CrmResourceParameters crmResourceParameters,
            string ControllerName)
        {
            var prevousPageLink = entityPagedList.HasPrevious ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, ControllerName) : null;

            var nextPageLink = entityPagedList.HasNext ?
                CrmUriHelpers.CreateCrmObjectRecourceUri(urlHelper, crmResourceParameters,
                ResourceUriType.NextPage, ControllerName) : null;

            var paginationMetadata = new
            {
                totalCount = entityPagedList.TotalCount,
                pageSize = entityPagedList.PageSize,
                currentPage = entityPagedList.CurrentPage,
                totalPages = entityPagedList.TotalPages,
                prevousPageLink,
                nextPageLink
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata);
        }
    }
}
