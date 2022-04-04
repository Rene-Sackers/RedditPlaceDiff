using Newtonsoft.Json;

namespace RedditPlaceDiff;

public class MapHistory
{
	[JsonProperty("file")]
	public string File { get; set; }

	[JsonProperty("reason")]
	public string Reason { get; set; }

	[JsonProperty("date")]
	public object Date { get; set; }
}

public class Brands
{
	[JsonProperty("nodeheadlessV7")]
	public int NodeheadlessV7 { get; set; }

	[JsonProperty("userscriptV20")]
	public int UserscriptV20 { get; set; }

	[JsonProperty("nodeheadlessV6")]
	public int NodeheadlessV6 { get; set; }

	[JsonProperty("userscriptV23")]
	public int UserscriptV23 { get; set; }

	[JsonProperty("nodeheadlessV5")]
	public int NodeheadlessV5 { get; set; }

	[JsonProperty("nodeheadlessV4")]
	public int NodeheadlessV4 { get; set; }

	[JsonProperty("PlaceNLpythonV2")]
	public int PlaceNLpythonV2 { get; set; }

	[JsonProperty("nodeheadlessV3")]
	public int NodeheadlessV3 { get; set; }

	[JsonProperty("unknown")]
	public int Unknown { get; set; }

	[JsonProperty("userscriptV19")]
	public int UserscriptV19 { get; set; }

	[JsonProperty("userscriptV17")]
	public int UserscriptV17 { get; set; }

	[JsonProperty("userscriptV18")]
	public int UserscriptV18 { get; set; }

	[JsonProperty("userscriptV16")]
	public int UserscriptV16 { get; set; }
}

public class ApiModel
{
	[JsonProperty("rawConnectionCount")]
	public int RawConnectionCount { get; set; }

	[JsonProperty("connectionCount")]
	public int ConnectionCount { get; set; }

	[JsonProperty("currentMap")]
	public string CurrentMap { get; set; }

	[JsonProperty("mapHistory")]
	public List<MapHistory> MapHistory { get; set; }

	[JsonProperty("brands")]
	public Brands Brands { get; set; }

	[JsonProperty("date")]
	public long Date { get; set; }
}