/*
    Assists with HATEOAS 
*/
using DemoCrm.Data.Entities;
using DemoCrm.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace DemoCrm.Data.Helpers
{
    public class CrmUriHelpers
    {
        public static IEnumerable<HypermediaLink> CreateActionLinksExpando(IUrlHelper urlHelper, 
            Guid id, string fields, string controllerName)
        {
            var links = new List<HypermediaLink>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new HypermediaLink(
                    urlHelper.Link($"Get{controllerName}", new { id }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(new HypermediaLink(
                    urlHelper.Link($"Get{controllerName}", new { id, fields }),
                    "self",
                    "GET"));
            }

            links.Add(new HypermediaLink(urlHelper.Link($"Create{controllerName}",
                null),
                $"create-{controllerName}",
                "POST"));

            links.Add(new HypermediaLink(urlHelper.Link($"PartiallyUpdate{controllerName}",
                new { id }),
                $"partially-update-{controllerName}",
                "PATCH"));

            links.Add(new HypermediaLink(urlHelper.Link($"FullyUpdate{controllerName}",
                new { id }),
                $"fully-update-{controllerName}",
                "PUT"));

            links.Add(new HypermediaLink(
                urlHelper.Link($"Delete{controllerName}", new { id, fields }),
                $"delete-{controllerName}",
                "DELETE"));

            return links;
        }

        //Creates links with Data shaping capabilities for a list of CrmUsers
        public static IEnumerable<HypermediaLink> CreateActionLinksExpandoList(IUrlHelper urlHelper,
            CrmResourceParameters crmResourceParameters,
            bool hasNext, bool hasPrevious, string controllerName)
        {
            var links = new List<HypermediaLink>();

            links.Add(
                new HypermediaLink(CreateCrmObjectRecourceUri(urlHelper, crmResourceParameters,
                ResourceUriType.Current, controllerName),
                "self", "GET"));

            if (hasNext)
                links.Add(
                new HypermediaLink(CreateCrmObjectRecourceUri(urlHelper, crmResourceParameters,
                ResourceUriType.NextPage, controllerName),
                "nextPage", "GET"));

            if (hasPrevious)
                links.Add(
                new HypermediaLink(CreateCrmObjectRecourceUri(urlHelper, crmResourceParameters,
                ResourceUriType.PreviousPage, controllerName),
                "previousPage", "GET"));

            return links;
        }

        public static string CreateCrmObjectRecourceUri(IUrlHelper urlHelper, CrmResourceParameters crmResourceParameters,
           ResourceUriType resourceUriType, string controllerName)
        {
            switch (resourceUriType)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link($"Get{controllerName}",
                      new
                      {
                          fields = crmResourceParameters.Fields,
                          orderBy = crmResourceParameters.OrderBy,
                          searchQuery = crmResourceParameters.SearchQuery,
                          sampleUser = crmResourceParameters.Type,
                          pageNumber = crmResourceParameters.PageNumber - 1,
                          pageSize = crmResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link($"Get{controllerName}",
                      new
                      {
                          fields = crmResourceParameters.Fields,
                          orderBy = crmResourceParameters.OrderBy,
                          searchQuery = crmResourceParameters.SearchQuery,
                          sampleUser = crmResourceParameters.Type,
                          pageNumber = crmResourceParameters.PageNumber + 1,
                          pageSize = crmResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link($"Get{controllerName}",
                    new
                    {
                        fields = crmResourceParameters.Fields,
                        orderBy = crmResourceParameters.OrderBy,
                        searchQuery = crmResourceParameters.SearchQuery,
                        sampleUser = crmResourceParameters.Type,
                        pageNumber = crmResourceParameters.PageNumber,
                        pageSize = crmResourceParameters.PageSize
                    });
            }
        }

        //public static IDictionary<IEnumerable<IDictionary<string, object>>, IEnumerable<HypermediaLink>>
        //    GetLinkedCollectionResource<T>(PagedList<T> crmEntities,
        //    IUrlHelper urlHelper, 
        //    CrmResourceParameters crmResourceParameters, 
        //    string ControllerName)
        //    where T : DemoCrmEntity 
        //{
        //    var links = CreateActionLinksExpandoList(urlHelper, crmResourceParameters,
        //            crmEntities.HasNext, crmEntities.HasPrevious, ControllerName);

        //    var shapedCrmUsersWithLinks = crmEntities.Select(crmUser =>
        //    {
        //        var crmUsersAsDictionary = crmUser as IDictionary<string, object>;
        //        var crmUserLinks =  CreateActionLinksExpando(urlHelper,
        //            (Guid)crmUsersAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

        //        crmUsersAsDictionary.Add("links", crmUserLinks);

        //        return crmUsersAsDictionary;
        //    });

        //    var linkedCollectionsResource = new
        //    {
        //        value = shapedCrmUsersWithLinks,
        //        links
        //    };

        //    var returnData = new Dictionary<IEnumerable<IDictionary<string, object>>, IEnumerable<HypermediaLink>>();
        //    returnData.Add(shapedCrmUsersWithLinks, links);

        //    return returnData;
        //}


        public static object
            GetLinkedCollectionResource<T>(PagedList<T> crmEntities,
            IUrlHelper urlHelper,
            CrmResourceParameters crmResourceParameters,
            string ControllerName)
            where T : DemoCrmEntity
        {
            var links = CreateActionLinksExpandoList(urlHelper, crmResourceParameters,
                    crmEntities.HasNext, crmEntities.HasPrevious, ControllerName);

            var shapedCrmUsersWithLinks = crmEntities.Select(crmUser =>
            {
                var crmUsersAsDictionary = crmUser as IDictionary<string, object>;
                var crmUserLinks = CreateActionLinksExpando(urlHelper,
                    (Guid)crmUsersAsDictionary["Id"], crmResourceParameters.Fields, ControllerName);

                crmUsersAsDictionary.Add("links", crmUserLinks);

                return crmUsersAsDictionary;
            });

            //var linkedCollectionsResource = new
            //{
            //    value = shapedCrmUsersWithLinks,
            //    links
            //};

            //var returnData = new Dictionary<IEnumerable<IDictionary<string, object>>, IEnumerable<HypermediaLink>>();
            //returnData.Add(shapedCrmUsersWithLinks, links);

            return new
            {
                value = shapedCrmUsersWithLinks,
                links
            };
        }
    }
}
