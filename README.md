# Cheese

A new generation of project scaffolding tools to make your development process more effective with half the effort.

## Installation

Install `Cheese` by dotnet tool:

```shell
dotnet tool install --global Crequency.Cheese
```

## Usage

You can use `setup` command from `Cheese` to clone a repo and setup it.

For example:

```shell
cheese setup Crequency/KitX.github
# We use protocol suffix including "github", "gitlab", "gitee"
# The default protocol is github, so you can execute: `cheese setup Crequency/KitX`
```

This command will clone `Crequency/KitX` repo from github, and check if there is `.cheese` folder.

With `.cheese` folder exists, the setup action will be execute automatically.

### Update your project

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
