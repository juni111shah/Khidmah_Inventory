# Lottie icons (free – LottieFiles-style)

Sidebar and nav icons use **Lottie** animations loaded from this folder.

## Adding free animations

1. Go to [LottieFiles free animations](https://lottiefiles.com/free-animations).
2. Pick an icon (e.g. “dashboard”, “settings”, “chart”).
3. Download as **Lottie JSON**.
4. Save the file here (e.g. `dashboard.json`, `settings.json`).
5. In `src/app/core/services/lottie-icons.service.ts`, add or update the mapping, e.g.:
   - `speedometer2: 'dashboard.json'`
   - `gear: 'settings.json'`

All animations in this folder are used only for UI icons and are loaded on demand.
