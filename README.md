<p align="center">
  <a href="#" target="_blank" rel="noopener noreferrer">
    <img width="128" src="https://raw.githubusercontent.com/Crequency/Cheese/main/Cheese/icon.png" alt="Cheese Logo"/>
  </a>
</p>

<h1 align="center">Cheese</h1>

<p align="center">
  <img alt="GitHub License" src="https://img.shields.io/github/license/Crequency/Cheese"/>
  <img alt="GitHub workflow status - mirror" src="https://img.shields.io/github/actions/workflow/status/Crequency/Cheese/mirror.yml"/>
  <img alt="Nuget version" src="https://img.shields.io/nuget/v/Cheese">
  <img alt="Nuget download" src="https://img.shields.io/nuget/dt/Cheese">
  <img alt="GitHub issues" src="https://img.shields.io/github/issues/Crequency/Cheese">
  <img alt="GitHub pull requests" src="https://img.shields.io/github/issues-pr/Crequency/Cheese">
</p>

<p align="center">
  <img src="https://profile-counter.glitch.me/Crequency-Cheese/count.svg">
</p>

# About

A new generation of project scaffolding tool to make your development process more effective with half the effort.

## Installation

Install `Cheese` by dotnet tool:

```shell
dotnet tool install --global Crequency.Cheese
```

## Usage

### Initialize

You can use `init` command from `Cheese` to clone a repo and setup it.

For example:

```shell
cheese init Crequency/KitX.github
# We use protocol suffix including "github", "gitlab", "gitee"
# The default protocol is github, so you can execute: `cheese setup Crequency/KitX`
```

This command will clone `Crequency/KitX` repo from github, and check if there is `.cheese` folder.

With `.cheese` folder exists, the init action will be execute automatically.

> In any folder, execute pure `cheese init` will only create `.cheese` folder.

### Setup

### ToDo Commands

- commit
- publish
- i18n
- doctor
- test
- docs
- config

## Update your project

Your repo should have a `.cheese` folder in the root directory like:

```text
+ project
  + .cheese
  + src
  + docs
    README.md
    LICENSE
```

You can create this folder by `cheese init` command.

## Contributors

[![Contributors](https://contrib.rocks/image?repo=Crequency/Cheese)](https://github.com/Crequency/Cheese/graphs/contributors)

# Star History

[![Star History Chart](https://api.star-history.com/svg?repos=Crequency/Cheese&type=Timeline)](https://star-history.com/#Crequency/Cheese&Timeline)
