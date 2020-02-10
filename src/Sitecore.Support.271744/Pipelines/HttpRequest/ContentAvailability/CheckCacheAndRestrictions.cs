using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Caching;
using Sitecore.Data.Fields;
using Sitecore.Pipelines.HttpRequest;

namespace Sitecore.Support.Pipelines.HttpRequest.ContentAvailability
{
    public class CheckCacheAndRestrictions : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            if (Sitecore.Context.Item != null)
            {
                var currentVersion = Sitecore.Context.Item;
                var previousVersion = currentVersion;
                if (currentVersion.Versions.Count > 1)
                {
                    var previousVersions = currentVersion.Versions.GetOlderVersions();
                    previousVersion =
                        previousVersions
                            [previousVersions.Length - 2]; // 1 is offset for the last entry, 2 is offset for second to last, array will contain   current version too
                }

                var now = DateTime.Now.ToUniversalTime();
                DateField sunset = previousVersion.Fields[FieldIDs.ValidTo];
                DateField sunrise = currentVersion.Fields[FieldIDs.ValidFrom];
                if (now > sunset.DateTime && now > sunrise.DateTime)
                {
                    HtmlCache cache = CacheManager.GetHtmlCache(Context.Site);
                    cache.Clear();
                }
            }
        }
    }
}