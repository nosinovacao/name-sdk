namespace NAME.Elasticsearch
{
    internal class ElasticResponseJson
    {
        public string name { get; set; }
        public string cluster_name { get; set; }
        public ElasticVersion version { get; set; }
    }

    internal class ElasticVersion
    {
        public string number { get; set; }
        public string lucene_version { get; set; }
    }
}