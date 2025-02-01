using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Models.General;

namespace PrintSiteBuilder.SiteItem
{
    public class ServiceWorker
    {
        public void CreateServiceWorker()
        {
            var cacheFileList = new List<string>
        {
            "/index.html",
            "/manifest.json",
            "/service-worker.js",
            "/web.config"
        };

            cacheFileList.AddRange(Directory.GetFiles(GlobalConfig.WebpDir, "*.webp").Select(path => $@"/webp/{Path.GetFileName(path)}"));
            cacheFileList.AddRange(Directory.GetFiles(GlobalConfig.WebpMobileDir, "*.webp").Select(path => $@"/webp-mobile/{Path.GetFileName(path)}"));
            cacheFileList.AddRange(Directory.GetFiles(GlobalConfig.WebpSmallDir, "*.webp").Select(path => $@"/webp-small/{Path.GetFileName(path)}"));
            cacheFileList.AddRange(Directory.GetFiles(GlobalConfig.HtmlDir, "*.html").Select(path => $@"/page/{Path.GetFileName(path)}"));
            cacheFileList.AddRange(Directory.GetFiles(GlobalConfig.IconDir, "*.svg").Select(path => $@"/icon/{Path.GetFileName(path)}"));
            cacheFileList.AddRange(Directory.GetFiles(GlobalConfig.CssDir, "*.css").Select(path => $@"/css/{Path.GetFileName(path)}"));
            cacheFileList.AddRange(Directory.GetFiles(GlobalConfig.JsDir, "*.js").Select(path => $@"/js/{Path.GetFileName(path)}"));
            cacheFileList.AddRange(Directory.GetFiles(GlobalConfig.SitemapDir, "*.xml").Select(path => $@"/sitemap/{Path.GetFileName(path)}"));

            // JSON形式の文字列に変換
            var cacheListStr = string.Join(",\n", cacheFileList.Select(item => $"\"{item}\""));

            var script = $@"self.addEventListener('install', function (e) {{
                              e.waitUntil(
                                caches.open('my-pwa-cache').then(function (cache) {{
                                  return cache.addAll([
                                    {cacheListStr}
                                  ]);
                                }})
                              );
                            }});

                            self.addEventListener('fetch', function (event) {{
                              event.respondWith(
                                caches.match(event.request).then(function (response) {{
                                  return response || fetch(event.request);
                                }}))
                              );
                            }});";

            File.WriteAllText($@"{GlobalConfig.ItemDir}\service-worker.js", script);
        }
    }
}
