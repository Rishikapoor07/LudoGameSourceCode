# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [15.1.180001-pre.4] - 2022-08-08
### Changed
 - Adjusted the Windows and Mac VivoxNative library .meta files to enable `Load on Startup` so they get loaded in regardless of the build target of the editor. Notably, this caused compiler errors when entering Play Mode if the editor build target was iOS since our Mac library, which gets used for the editor alongside the Windows library, wasn't loaded yet.
### Fixed
 - Fixed an issue with various plugin `.meta` files not targeting their specific platform resulting in editor crashes when entering Play Mode if additional Vivox platform packages got installed.
 - Fixed an issue that would cause a compiler error if the Authentication package wasn't available to the ChatChannelSample. The Authentication package should get automatically pulled in when the ChatChannelSample is installed, but if it hadn't been, a compiler error would appear.

## [15.1.180001-pre.3] - 2022-07-13
### Added
 - Added VivoxConfig as a parameter to the VivoxService.Instance.Initialize(...) method so developers can configure the Vivox Client's settings on startup.
### Changed
 - Stopped removing minus ('-') characters from the Environment ID when appending it to the AccountId and ChannelId URIs. The Environment ID will now be appended verbatim.
### Fixed
 - Fixed an issue causing generated files from the Android platform to make it into the package resulting in methods that could not be resolved due to the APIs not existing in any other library.

## [15.1.180001-pre.2] - 2022-06-22
### Fixed
- Fixed a warning that would always throw when using custom credentials via InitializationOptions.SetVivoxCredentials and initializing the Vivox service.
- Fixed IEnvironmentId not being added as an optional dependency in VivoxPackageInitializer.cs causing erroneous and confusing warnings when initializing the Vivox service.

## [15.1.180001-pre.1] - 2022-06-03
### Fixed
- Fixed how we fetch the Authentication package's Environment ID to fix the Vivox package not working when using newer versions of Authentication.
- Fixed debug/local token generation when Test Mode is enabled in the Vivox service settings and the user is signed into Authentication.

## [15.1.180000-pre.1] - 2022-05-02
### Added
- Automatic Connection Recovery has been added to the Unity API, along with an example Connection Indicator added to the Chat Channel Sample.
### Changed
- Fixed a bug that left the suggestion to add the Authentication package for easy Vivox Logins even after the Authentication package had been imported.

## [15.1.170000-pre.1] - 2022-02-25
### Added
- Added a display name to constructs provided by the SDK when events fire for receiving a message, a transcribed message, or a buddy request.
### Changed
- Removed the option to provide custom credentials in the Vivox service window.
- Updated ChatChannelSample to demonstrate how to leverage both Authentication and a custom credential-based flow.

## [15.1.160000-pre.1] - 2021-10-20
### Fixed
- Fixed a bug that prevented users from switching to the custom credentials option in the Vivox service window when Unity Game Services was enabled.
- Improved the experience for developers using the Chat Channel Sample who are not using Unity Game Services.
- Enabled bitcode configuration for build output that prevented developers from archiving Xcode builds on iOS.
### Added
- Added a DeviceEnergy property to AudioOutputDevices.cs and AudioInputDevices.cs
### Changed
- Adjusted Vivox's default AVAudioSession values on iOS to provide improved volume-related behavior.

## [15.1.150002-pre.2] - 2021-10-20
### Fixed
- Chat Channel Sample was not pulling in its Authentication package dependency. The Authentication package is now added to the project if it is not already there.

## [15.1.150002-pre.1] - 2021-10-06
### Added
- For com.unity.services.vivox users, new Channel and Account classes should replace ChannelId and AccountId, respectively.
- Updated Debug Token Generation to be overridden with Auth JWT generation if the Authentication package is in use.
- Added a warning for Apple silicon builds which notes that Vivox does not work. macOS users must set their Unity target platform architecture to Intel 64-bit.
- Added an Editor script to broadcast a warning if a conflicting Unity Asset Store package is loaded in a project.
### Changed
- Updated the Chat Channel Sample to properly use VivoxSettings credentials.
- Modified channel session deletion to use channel state events. Connected channels can now be safely deleted when connected.
- Updated the Services menu to redirect to the Vivox Project Settings.
- Updated the code snippet example for signing in and joining an echo channel in the Vivox Unity SDK QuickStart documentation to use plain bools instead of checks against the enum.
- Revised various engine startup warnings.
- Shortened autogenerated file names.

## [15.1.150000] - 2020-08-02
### Added
- New Vivox Credential manager in Project/Settings/Services/Vivox
- Dependency on com.unity.services.core in our new Unity.Services.Vivox namespace
- New way to initialize Vivox allow side all other Game Services
- Leverage Unity Project Environments

## [5.15.0-preview] - 2020-06-03
### Changed
- First transition from our old Unity Asset Store Package to a UPM Package com.unity.services.vivox
