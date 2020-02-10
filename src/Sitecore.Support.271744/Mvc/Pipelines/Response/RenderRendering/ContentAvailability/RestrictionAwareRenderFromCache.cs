using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace Sitecore.Support.Mvc.Pipelines.Response.RenderRendering.ContentAvailability
{
    public class RestrictionAwareRenderFromCache: Sitecore.Mvc.Pipelines.Response.RenderRendering.RenderFromCache
    {
        public override void Process(RenderRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (!args.Rendered)
            {
                string cacheKey = args.CacheKey;
                if (args.Cacheable && !string.IsNullOrEmpty(cacheKey) && !this.ShouldUpdateCache(args) && this.Render(cacheKey, args.Writer, args))
                {
                    args.Rendered = true;
                    args.Cacheable = false;
                    args.UsedCache = true;
                }
            }
        }

        private bool ShouldUpdateCache(RenderRenderingArgs args)
        {
            var currentVersion = args.Rendering.Item;
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
                return true;
            }
            return false;
        }
    }
}