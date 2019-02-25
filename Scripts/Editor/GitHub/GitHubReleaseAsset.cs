using System;

namespace FVTC.LearningInnovations.Unity.Editor.GitHub
{
    [Serializable]
    public class GitHubReleaseAsset
    {
        public string url;
        public int id;
        public string node_id;
        public string name;
        public object label;
        public GitHubReleaseUploader uploader;
        public string content_type;
        public string state;
        public int size;
        public int download_count;
        public DateTime created_at;
        public DateTime updated_at;
        public string browser_download_url;
    }
}