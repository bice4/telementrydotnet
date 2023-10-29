using System.ComponentModel.DataAnnotations;

namespace MetricsPlayground;

public class KeyValueRequest
{
    [Required(ErrorMessage = $"{nameof(Key)} is required")]
    public string Key { get; set; }
    
    [Required(ErrorMessage = $"{nameof(Value)} is required")]
    public string Value { get; set; }

    public KeyValueRequest(string key, string value)
    {
        Key = key;
        Value = value;
    }
}