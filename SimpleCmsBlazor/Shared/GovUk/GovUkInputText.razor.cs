using Microsoft.AspNetCore.Components;
using Havit.Blazor.Components.Web;
using Microsoft.AspNetCore.Components.Web;

namespace SimpleCmsBlazor.Shared.GovUk;

public partial class GovUkInputText
{
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? InputAttributes { get; set; }

	[Parameter]
	public bool LargeLabel { get; set; } = false;

	[Parameter]
	public EventCallback<KeyboardEventArgs> OnKeyPress { get; set; }

	[Parameter]
	public string Suffix { get; set; } = string.Empty;

	[Parameter]
	public string Prefix { get; set; } = string.Empty;

	[Parameter]
	public string Label { get; set; } = string.Empty;

	[Parameter]
	public string LabelCssClass { get; set; } = "govuk-label--l";

	[Parameter]
	public string DivCssClass { get; set; } = string.Empty;

	private string _textboxValue = string.Empty;

	[Parameter]
	public string Value { get; set; } = string.Empty;

	[Parameter]
	public EventCallback<string> ValueChanged { get; set; }

	private async Task SetTextBoxValueAsync(string value)
	{
		if (value == _textboxValue) return;
		_textboxValue = value;
		if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(_textboxValue);
		if (OnKeyPress.HasDelegate) await OnKeyPress.InvokeAsync(_lastKeyPress);
	}

	[Parameter]
	public string Hint { get; set; } = string.Empty;

	[Parameter]
	public string Id { get; set; } = Guid.NewGuid().ToString("N");

	[Parameter]
	public string ErrorMessage { get; set; } = string.Empty;

	[Parameter]
	public InputType Type { get; set; } = InputType.Text;

	[Parameter]
	public string CssClass { get; set; } = string.Empty;

	[Parameter]
	public bool Disabled { get; set; }

	private KeyboardEventArgs? _lastKeyPress;

	public void KeyPress(KeyboardEventArgs args)
	{
		_lastKeyPress = args;
		if (OnKeyPress.HasDelegate) OnKeyPress.InvokeAsync(_lastKeyPress);
	}

	protected override void OnParametersSet()
	{
		if (Value == _textboxValue) return;
		_textboxValue = Value;
	}
}