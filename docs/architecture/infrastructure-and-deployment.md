# Infrastructure and Deployment

## Infrastructure as Code

**Not Applicable** - As a desktop application with no cloud components, there is no infrastructure provisioning required. Application deployment follows traditional Windows application distribution patterns.

## Deployment Strategy

**Strategy:** Manual distribution via GitHub Releases with optional Microsoft Store publishing (Phase 2)

**Build Platform:** GitHub Actions

**Pipeline Configuration:** `.github/workflows/build.yml`

**Deployment Artifacts:**
- Self-contained MSIX installer package (includes .NET 8 runtime)
- Portable ZIP archive for manual deployment
- Version-specific release notes generated from changelog

**Build Pipeline Steps:**
1. **Build Stage:** Compile application with Release configuration
2. **Test Stage:** Run xUnit test suite with code coverage reporting
3. **Package Stage:** Generate MSIX installer with code signing certificate
4. **Artifact Stage:** Upload installer and ZIP archive to GitHub Releases
5. **Validation Stage:** Automated smoke tests on clean Windows VM

## Environments

- **Development:** Local developer workstations running VS Code + C# Dev Kit, localhost debugging with F5 launch configuration
- **CI/Build:** GitHub Actions runners (Windows 2022 image), automated builds on every commit to `main` branch
- **Release:** GitHub Releases page for stable builds, tagged with semantic versioning (e.g., `v1.0.0`)
- **User Deployment:** End-user workstations running Windows 10 21H2+ or Windows 11, Standard User accounts without elevation

**Note:** No traditional "staging" or "production" environments exist since this is a client-side application. Testing occurs in CI and developer machines before release publication.

## Environment Promotion Flow

```
Developer Branch → Pull Request → CI Build + Test → main Branch → Tagged Release → GitHub Releases → User Download
```

**Detailed Flow:**
1. Developer creates feature branch, implements changes locally
2. Developer runs local tests (`dotnet test`), ensures build succeeds
3. Developer opens Pull Request to `main` branch
4. GitHub Actions CI runs automated build, test, and MSIX packaging
5. Code review + CI pass → PR merged to `main`
6. Maintainer creates Git tag (e.g., `v1.1.0`) when ready for release
7. Tag creation triggers GitHub Actions release workflow
8. Release workflow builds signed MSIX, uploads to GitHub Releases with changelog
9. Users download installer from GitHub Releases page
10. Users run MSIX installer or extract ZIP for portable deployment

## Rollback Strategy

**Primary Method:** Users reinstall previous version from GitHub Releases archive

**Trigger Conditions:**
- Critical bug reports affecting core enumeration or clipboard functionality
- Compatibility issues discovered on specific Windows versions
- Security vulnerabilities requiring immediate remediation

**Rollback Process:**
1. Maintainer marks current release as "Pre-release" on GitHub with warning banner
2. Create hotfix branch from previous stable tag
3. Apply fix, increment patch version (e.g., `v1.0.1`)
4. Follow standard release process for hotfix version
5. Mark hotfix as "Latest Release" on GitHub
6. Post announcement in README and GitHub Discussions

**Recovery Time Objective:** Users can manually rollback in <5 minutes by downloading and installing previous version from GitHub Releases. No data loss since application has no persistent state.

**Note:** Unlike web applications, desktop app rollback is user-initiated. Automated rollback not feasible for client-side software.
