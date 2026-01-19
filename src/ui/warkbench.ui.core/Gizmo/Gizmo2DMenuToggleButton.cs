using System;
using System.Collections.Generic;
using System.Linq;


namespace warkbench.src.ui.core.Gizmo;

internal record MultiStateToggleCreateInfo
(
    string Id,
    string IconName,
    string Tooltip,
    double Scale = 1.0d,
    bool IsDefault = false,
    bool IsEnabled = true,
    Action? OnSelected = null
);

internal class Gizmo2DMenuToggleButton : Gizmo2DMenuButton
{
    public Gizmo2DMenuToggleButton(
        double degrees,
        double rotation,
        IEnumerable<MultiStateToggleCreateInfo>? createInfo)
        : base(
            iconName: GetFallbackIconName(createInfo),
            tooltip:  GetFallbackTooltip(createInfo),
            degrees:  degrees,
            rotation: rotation,
            scale:    GetFallbackScale(createInfo))
    {
        _states = (createInfo ?? []).ToList();

        _currentState =
            _states.FirstOrDefault(s => s is { IsDefault: true, IsEnabled: true }) ??
            _states.FirstOrDefault(s => s.IsEnabled) ??
            _states.FirstOrDefault();
    }

    private static string GetFallbackIconName(IEnumerable<MultiStateToggleCreateInfo>? createInfo)
        => createInfo?.FirstOrDefault()?.IconName ?? string.Empty;

    private static string GetFallbackTooltip(IEnumerable<MultiStateToggleCreateInfo>? createInfo)
        => createInfo?.FirstOrDefault()?.Tooltip ?? string.Empty;
    
    private static double GetFallbackScale(IEnumerable<MultiStateToggleCreateInfo>? createInfo)
        => createInfo?.FirstOrDefault()?.Scale ?? 1.0d;

    private void AdvanceState()
    {
        if (_states.Count == 0)
            return;

        var startIndex = _currentState is null ? -1 : _states.IndexOf(_currentState);
        
        for (var i = 1; i <= _states.Count; i++)
        {
            var idx  = (startIndex + i) % _states.Count;
            var next = _states[idx];

            if (!next.IsEnabled)
                continue;

            _currentState = next;
            next.OnSelected?.Invoke();
            break;
        }
    }
    
    public override string IconName => _currentState?.IconName ?? base.IconName;
    public override string Tooltip  => _currentState?.Tooltip  ?? base.Tooltip;
    public override double Scale => _currentState?.Scale ?? base.Scale;

    /// <summary>
    /// Toggles to the next enabled state on press (rising edge).
    /// Note: If you prefer toggling on release/click, move AdvanceState() into your click/invoke logic instead.
    /// </summary>
    public override bool IsPressed
    {
        get => base.IsPressed;
        set
        {
            var wasPressed = base.IsPressed;
            base.IsPressed = value;

            if (!wasPressed && value)
                AdvanceState();
        }
    }

    private readonly List<MultiStateToggleCreateInfo> _states;
    private MultiStateToggleCreateInfo? _currentState;
}
