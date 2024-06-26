<p align="center">
  <a href="#" target="_blank" rel="noopener noreferrer">
    <img width="128" src="https://raw.githubusercontent.com/Crequency/Cheese/main/Cheese/icon.png" alt="Cheese Logo"/>
  </a>
</p>

<h1 align="center">Cheese</h1>

<p align="center">
  <img alt="GitHub License" src="https://img.shields.io/github/license/Crequency/Cheese"/>
  <img alt="GitHub workflow status - mirror" src="https://img.shields.io/github/actions/workflow/status/Crequency/Cheese/mirror.yml?label=mirror"/>
  <img alt="Nuget version" src="https://img.shields.io/nuget/v/Cheese">
  <img alt="Nuget download" src="https://img.shields.io/nuget/dt/Cheese">
  <img alt="GitHub issues" src="https://img.shields.io/github/issues/Crequency/Cheese">
  <img alt="GitHub pull requests" src="https://img.shields.io/github/issues-pr/Crequency/Cheese">
</p>

<p align="center">
  <img src="https://profile-counter.glitch.me/Crequency-Cheese/count.svg" alt="count">
</p>

# About

A new generation of project scaffolding tool to make your development process more effective with half the effort.

## Installation

Install `Cheese` by dotnet tool:

```shell
dotnet tool install cheese --global 
```

Update `Cheese` by dotnet tool:

```shell
dotnet tool update cheese -g
```

## Usage

### Initialize (In Design)

Verb: `init`

You can use `init` command from `Cheese` to clone a repo and setup it.

For example:

```shell
cheese init Crequency/KitX.github
# We use protocol suffix including "github", "gitlab", "gitee"
# The default protocol is github, so you can execute: `cheese init Crequency/KitX`
```

This command will clone `Crequency/KitX` repo from GitHub, and check if there is `.cheese` folder.

With `.cheese` folder exists, the init action will be executed automatically.

> In any folder, execute pure `cheese init` will only create `.cheese` folder.

### Setup

Verb: `setup`

- `--reference` `[bool]`: include this argument to set up reference
  - `cheese setup --reference` = `cheese reference --setup`

### Reference

Verb: `reference`, Alias: [ `refer`, `ref` ]

- `--status` `[bool]`: use this to show references status
- `--update` `[bool]`: use this to update references
  - `--fetch` `[bool]`: include this to fetch remote
- `--setup` `[bool]`: use this to set up references
  - `--convert-ssl-link-to-https-link` `[bool]`: include this to clone with `https` styled link

Cheese will run `.cheese/references.cs` to get references list.

### ToDo Commands

- commit
- doctor
- test
- docs
- config

## Update your project

Your repo should have a `.cheese` folder in the root directory like:

```text
+ project
  + .cheese
    + scripts
    \ references.cs
  + src
  + docs
    README.md
    LICENSE
```

You can create this folder by `cheese init` command.

## Contributors

[![Contributors](https://contrib.rocks/image?repo=Crequency/Cheese)](https://github.com/Crequency/Cheese/graphs/contributors)

# Star History

[![Star History Chart](https://starchart.cc/Crequency/Cheese.svg?variant=adaptive)](https://starchart.cc/Crequency/Cheese)

# Thanks to

<p align="center">
  <a href="https://www.jetbrains.com/" target="_blank" rel="noopener noreferrer">
    <img width="128" src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg" alt="JetBrains Logo (Main) logo">
  </a>
</p>

<p align="center">
    Thanks to the great tools from <a href="https://www.jetbrains.com/" target="_blank">JetBrains</a>, we can turn our ideas into reality.
</p>
