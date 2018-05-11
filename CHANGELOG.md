# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## v1.1.1 - 2018-02-22
### Fixed
* Fix XPath connection string resolver incorrectly resolving the connection string when there were more XML elements after the provided XPath.

## v1.1.0 - 2017-11-09
### Fixed
* Fix tab character '/t' breaking dependencies.json parsing. [(#10)](https://github.com/nosinovacao/name-sdk/issues/9)
* Fix the table ui endpoint failing to load the manifest behind a HTTPS proxy. [(#9)](https://github.com/nosinovacao/name-sdk/issues/9)
* Fix NAME not retrying to fetch a service dependency manifest from the correct endpoint when the dependency returned a successful status code. [(#8)](https://github.com/nosinovacao/name-sdk/issues/8)
### Added
* A relevant error message is now shown when a service dependency does not have NAME installed. [(#14)](https://github.com/nosinovacao/name-sdk/issues/14)
* Support for wildcards in the maximum version of dependencies. [(#11)](https://github.com/nosinovacao/name-sdk/issues/11)
* Support for Elasticsearch version resolving. [(#19)](https://github.com/nosinovacao/name-sdk/issues/19)
* A status field for each dependency in the manifest that describes its error level. [(#24)](https://github.com/nosinovacao/name-sdk/issues/24)
* Startup healthcheck is now performed asynchronously when `ThrowOnDependenciesFail` is set to false. [(#4)](https://github.com/nosinovacao/name-sdk/issues/19)

## v1.0.0 - 2017-07-07
* Initial open source release.


[Unreleased]: https://github.com/nosinovacao/name-sdk/compare/v1.1.0...HEAD
