# Cheese

A util of `KitX Project` that helps to manage repository, fetch codes and develop plugins.

## Installation

Install Cheese by dotnet tool:

```shell
dotnet tool install --global Crequency.Cheese
```

## Usage

`Cheese` support develop KitX Project Components and KitX plugins.

Cheese will detect plugin project type automatically.

- When you develop a KitX Project Components, `KitX Project` commands will be available.
- When you develop a KitX Plugin, `Plugins Development` commands will be available.

### KitX Project

- `init` - Init `KitX Project` repo with submodules from [GitHub](https://github.com/Crequency/KitX) or other sources.
    - After repo cloned, Cheese will requires you to select which fields you want to develop with.
- `build` - Build the repository.
- `clean` - Clean the repository.
- `commit` - Commit the changes to the current Git repository. This command will use Crequency commit message format.
- `fetch` - Fetch the subpackages required by the repository recursively.
- `publish` - Publish the repository to the specified directory. This command will run `fetch` and `build` before publishing. Then run publish scripts if exists.
- `test` - Run tests for the repository.
- `doctor` - Check and try to fix your development environment.
- `i18n` - i18n related command.

For `build`, `clean`, `commit`, `fetch`, `publish`, `test`, `doctor`, `i18n` commands you can append field name like: [dashboard, mobile, installer, website, ...]

### Plugins Development

- `create` - Create a new plugin.
- `attach` - Attach to a project file and enter development mode.
- `pack` - Pack the plugin to a KXP file.

### Tool Commands

- `unpack` - Unpack [`.kxp`, `.kxps`, `.kxpc`, `.kxpcs`] file to plugin files or plugins files.

## Profile

Cheese will use the profile file to manage the plugin.

The profile file is a JSON file in the root directory of the plugin repo.

The KitX plugin profile file is named `plugin.json`.

## Contributors

[![Contributors](https://contrib.rocks/image?repo=Crequency/Cheese)](https://github.com/Crequency/Cheese/graphs/contributors)
