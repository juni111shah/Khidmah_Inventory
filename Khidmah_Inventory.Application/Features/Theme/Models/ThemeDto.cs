namespace Khidmah_Inventory.Application.Features.Theme.Models;

public class ThemeDto
{
    // Branding
    public string LogoUrl { get; set; } = string.Empty;
    public string LogoHeight { get; set; } = "40px";

    // Colors
    public string PrimaryColor { get; set; } = "#2196F3";
    public string SecondaryColor { get; set; } = "#FF9800";
    public string AccentColor { get; set; } = "#4CAF50";
    public string BackgroundColor { get; set; } = "#F5F5F5";
    public string SurfaceColor { get; set; } = "#FFFFFF";
    public string TextColor { get; set; } = "#212121";
    public string TextSecondaryColor { get; set; } = "#757575";

    // Animations
    public bool AnimationsEnabled { get; set; } = true;
    public string AnimationSpeed { get; set; } = "normal";
    public int TransitionDuration { get; set; } = 300;

    // Buttons
    public string ButtonStyle { get; set; } = "raised";
    public string ButtonBorderRadius { get; set; } = "8px";
    public string ButtonPadding { get; set; } = "12px 24px";

    // Cards
    public string CardStyle { get; set; } = "elevated";
    public string CardBorderRadius { get; set; } = "12px";
    public int CardElevation { get; set; } = 2;
    public string CardShadow { get; set; } = "0 2px 8px rgba(0,0,0,0.1)";

    // Layout
    public string BorderRadius { get; set; } = "8px";
    public string Spacing { get; set; } = "16px";
}

