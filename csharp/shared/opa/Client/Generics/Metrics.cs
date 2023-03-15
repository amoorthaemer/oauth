using Newtonsoft.Json;

namespace OpenPolicyAgent.Common.Client.Generics;

public class Metrics
{
    [JsonProperty("timer_rego_input_parse_ns")]
    public long InputParseInNanoSecs { get; set; }

    [JsonProperty("timer_rego_query_parse_ns")]
    public long QueryParseInNanoSecs { get; set; }

    [JsonProperty("timer_rego_query_compile_ns")]
    public long QueryCompileInNanoSecs { get; set; }

    [JsonProperty("timer_rego_query_eval_ns")]
    public long QueryEvaluationInNanoSecs { get; set; }

    [JsonProperty("timer_rego_module_parse_ns")]
    //: time taken (in nanoseconds) to parse the input policy module.
    public long PolicyParseInNanoSecs { get; set; }

    [JsonProperty("timer_rego_module_compile_ns")]
    public long PolicyCompileInNanoSecs { get; set; }

    [JsonProperty("timer_server_handler_ns")]
    public long HandleRequestInNanoSecs { get; set; }

    [JsonProperty("counter_server_query_cache_hit")]
    public long NrOfServerQueryCacheHits { get; set; }
}
