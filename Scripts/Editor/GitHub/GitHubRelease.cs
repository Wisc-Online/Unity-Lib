using System;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Editor.GitHub
{
    [Serializable]
    public class GitHubRelease
    {
        public string url;
        public string assets_url;
        public string upload_url;
        public string html_url;
        public int id;
        public string node_id;
        public string tag_name;
        public string target_commitish;
        public string name;
        public bool draft;
        public GitHubReleaseAuthor author;
        public bool prerelease;
        public DateTime created_at;
        public DateTime published_at;
        public GitHubReleaseAsset[] assets;
        public string tarball_url;
        public string zipball_url;
        public string body;

        public static GitHubRelease Download(string url)
        {
            return Download(url, null);
        }

        public static GitHubRelease Download(string url, Action<float> downloadProgressCallback)
        {
            WWW www = new WWW(url);

            while (!www.isDone)
            {
                if (downloadProgressCallback != null)
                    downloadProgressCallback(www.progress);
            }

            if (www.error == null)
            {
                var release = JsonUtility.FromJson<GitHubRelease>(www.text);

                return release;
            }
            else
            {
                Debug.LogError(www.error);
                return null;
            }
        }
    }
}