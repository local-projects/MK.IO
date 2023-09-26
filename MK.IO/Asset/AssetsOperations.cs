﻿using MK.IO.Models;
using Newtonsoft.Json;
using System.Web;

namespace MK.IO.Asset
{
    internal class AssetsOperations : IAssetsOperations
    {
        private const string AssetsApiUrl = MKIOClient.AssetsApiUrl;
        private const string AssetApiUrl = AssetsApiUrl + "/{1}";
        private const string AssetListStreamingLocatorsApiUrl = AssetApiUrl + "/listStreamingLocators";
        private const string AssetListTracksAndDirectoryApiUrl = AssetApiUrl + "/storage/";

        /// <summary>
        /// Gets a reference to the AzureMediaServicesClient
        /// </summary>
        private MKIOClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the AssetsOperations class.
        /// </summary>
        /// <param name='client'>
        /// Reference to the service client.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a required parameter is null
        /// </exception>
        internal AssetsOperations(MKIOClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            Client = client;
        }

        /// <summary>
        /// Retrieves a list of assets in the instance.
        /// </summary>
        /// <param name="orderBy">Specifies the key by which the result collection should be ordered.</param>
        /// <param name="top">Specifies a non-negative integer that limits the number of items returned from a collection. The service returns the number of available items up to but not greater than the specified value top.</param>
        /// <returns></returns>
        public List<AssetSchema> List(string? orderBy = null, int? top = null)
        {
            Task<List<AssetSchema>> task = Task.Run(async () => await ListAsync(orderBy, top));
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieves a list of assets in the instance.
        /// </summary>
        /// <param name="orderBy">Specifies the key by which the result collection should be ordered.</param>
        /// <param name="top">Specifies a non-negative integer that limits the number of items returned from a collection. The service returns the number of available items up to but not greater than the specified value top.</param>
        /// <returns></returns>
        public async Task<List<AssetSchema>> ListAsync(string? orderBy = null, int? top = null)
        {
            var url = Client.GenerateApiUrl(AssetsApiUrl);
            url = MKIOClient.AddParametersToUrl(url, "$orderby", orderBy);
            url = MKIOClient.AddParametersToUrl(url, "$top", top != null ? ((int)top).ToString() : null);
            string responseContent = await Client.GetObjectContentAsync(url);
            return JsonConvert.DeserializeObject<AssetListResponseSchema>(responseContent, ConverterLE.Settings).Value;
        }

        /// <summary>
        /// Retrieves a list of assets in the instance.
        /// </summary>
        /// <param name="orderBy">Specifies the key by which the result collection should be ordered.</param>
        /// <param name="top">Specifies a non-negative integer that limits the number of items returned from a collection. The service returns the number of available items up to but not greater than the specified value top.</param>
        /// <returns></returns>
        public PagedResult<AssetSchema> ListAsPage(string? orderBy = null, int? top = null)
        {
            Task<PagedResult<AssetSchema>> task = Task.Run(async () => await ListAsPageAsync(orderBy, top));
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieves a list of assets in the instance.
        /// </summary>
        /// <param name="orderBy">Specifies the key by which the result collection should be ordered.</param>
        /// <param name="top">Specifies a non-negative integer that limits the number of items returned from a collection. The service returns the number of available items up to but not greater than the specified value top.</param>
        /// <returns></returns>
        public async Task<PagedResult<AssetSchema>> ListAsPageAsync(string? orderBy = null, int? top = null)
        {
            var url = Client.GenerateApiUrl(AssetsApiUrl);
            url = MKIOClient.AddParametersToUrl(url, "$orderby", orderBy);
            url = MKIOClient.AddParametersToUrl(url, "$top", top != null ? ((int)top).ToString() : null);
            string responseContent = await Client.GetObjectContentAsync(url);

            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);
            string? nextPageLink = null;

            nextPageLink = responseObject["@odata.nextLink"];

            return new PagedResult<AssetSchema>
            {
                NextPageLink = HttpUtility.UrlDecode(nextPageLink),
                Results = JsonConvert.DeserializeObject<AssetListResponseSchema>(responseContent, ConverterLE.Settings).Value
            };
        }

        /// <summary>
        /// Retrieves a list of assets in the instance.
        /// </summary>
        /// <param name="nextPageLink">Next page link.</param>
        /// <returns></returns>
        public PagedResult<AssetSchema> ListAsPageNext(string? nextPageLink)
        {
            Task<PagedResult<AssetSchema>> task = Task.Run(async () => await ListAsPageNextAsync(nextPageLink));
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieves a list of assets in the instance.
        /// </summary>
        /// <param name="nextPageLink">Next page link.</param>
        /// <returns></returns>
        public async Task<PagedResult<AssetSchema>> ListAsPageNextAsync(string? nextPageLink)
        {
            var url = Client.BaseUrl.Substring(0, Client.BaseUrl.Length - 1) + nextPageLink;
            string responseContent = await Client.GetObjectContentAsync(url);

            dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

            nextPageLink = responseObject["@odata.nextLink"];

            return new PagedResult<AssetSchema>
            {
                NextPageLink = HttpUtility.UrlDecode(nextPageLink),
                Results = JsonConvert.DeserializeObject<AssetListResponseSchema>(responseContent, ConverterLE.Settings).Value
            };
        }

        public AssetSchema Get(string assetName)
        {
            return GetAsync(assetName).GetAwaiter().GetResult();
        }

        public async Task<AssetSchema> GetAsync(string assetName)
        {
            var url = Client.GenerateApiUrl(AssetApiUrl, assetName);
            string responseContent = await Client.GetObjectContentAsync(url);
            return JsonConvert.DeserializeObject<AssetSchema>(responseContent, ConverterLE.Settings);
        }

        public AssetSchema CreateOrUpdate(string assetName, string containerName, string storageName, string description = null)
        {
            Task<AssetSchema> task = Task.Run(async () => await CreateOrUpdateAsync(assetName, containerName, storageName, description));
            return task.GetAwaiter().GetResult();
        }

        public async Task<AssetSchema> CreateOrUpdateAsync(string assetName, string containerName, string storageName, string description = null)
        {
            var url = Client.GenerateApiUrl(AssetApiUrl, assetName);
            AssetSchema content = new()
            {
                Name = assetName,
                Properties = new AssetProperties
                {
                    Container = containerName,
                    Description = description,
                    StorageAccountName = storageName
                }
            };

            string responseContent = await Client.CreateObjectAsync(url, JsonConvert.SerializeObject(content, Formatting.Indented));
            return JsonConvert.DeserializeObject<AssetSchema>(responseContent, ConverterLE.Settings);
        }

        public void Delete(string assetName)
        {
            Task.Run(async () => await DeleteAsync(assetName));
        }

        public async Task DeleteAsync(string assetName)
        {
            var url = Client.GenerateApiUrl(AssetApiUrl, assetName);
            await Client.ObjectContentAsync(url, HttpMethod.Delete);
        }

        public List<AssetStreamingLocator> ListStreamingLocators(string assetName)
        {
            Task<List<AssetStreamingLocator>> task = Task.Run(async () => await ListStreamingLocatorsAsync(assetName));
            return task.GetAwaiter().GetResult();
        }

        public async Task<List<AssetStreamingLocator>> ListStreamingLocatorsAsync(string assetName)
        {
            var url = Client.GenerateApiUrl(AssetListStreamingLocatorsApiUrl, assetName);
            string responseContent = await Client.GetObjectContentAsync(url);
            return AssetListStreamingLocators.FromJson(responseContent).StreamingLocators;
        }

        public AssetStorageResponseSchema ListTracksAndDirListing(string assetName)
        {
            var task = Task.Run(async () => await ListTracksAndDirListingAsync(assetName));
            return task.GetAwaiter().GetResult();
        }

        public async Task<AssetStorageResponseSchema> ListTracksAndDirListingAsync(string assetName)
        {
            var url = Client.GenerateApiUrl(AssetListTracksAndDirectoryApiUrl, assetName);
            string responseContent = await Client.GetObjectContentAsync(url);
            return JsonConvert.DeserializeObject<AssetStorageResponseSchema>(responseContent, ConverterLE.Settings);
        }
    }
}
