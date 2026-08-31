# Handoff Document: XR Sandbox Code Exploration

## Task Overview
Exploring and explaining the code structure of a Unity-based Extended Reality (XR) sandbox project (`xr-sandbox-app`). The user asked "look at the code and pls explain test" followed by "any other thoughts?" seeking additional insights.

## Current Status
**Exploratory analysis complete.** Reviewed codebase architecture, key implementations, documentation, and identified important observations. No specific implementation goal set beyond understanding. Task is exploratory and ongoing for user follow-up questions.

## User Preferences & Constraints
- **Demo-isolated architecture**: All development happens in `/Assets/App/` only (per `.antigravity/rules.md`)
- **XR Interaction Toolkit 3.2.1 ONLY**: Do NOT use Meta XR SDK, Oculus SDK, or other platform-specific XR SDKs
- **Reference external projects**: Can learn from samples/external sources but DO NOT modify anything outside `/Assets/App/`
- **Code standards**: Microsoft C# conventions, Unity naming conventions, performance best practices
- **No explicit constraints** provided beyond the above

## Important Decisions
| Decision | Source | Status |
|----------|--------|--------|
| Demo-isolated architecture interpretation | `.antigravity/rules.md` | Rule-based constraint |
| Focus on `/Assets/App/` for development | `.antigravity/rules.md` | Rule-based constraint |
| Mixed SDK observation (XRIT vs Meta Quest) | Code inspection | Documented concern - **NOT a violation** |

**Note**: The project documentation mentions Meta XR SDK migration plans, but the actual rules explicitly state to use **only Unity XR Interaction Toolkit 3.2.1**. This is by design - the migration docs are for future consideration.