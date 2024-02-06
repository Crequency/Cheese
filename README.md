# Cheese

A util of Crequency that helps to manage repository, fetch codes and develop plugins.

## Installation

Install Cheese by dotnet tool:

```bash
dotnet tool install --global Crequency.Cheese
```

## Usage

Cheese support develop Crequency software and KitX plugins. Cheese will detect package type automatically. When you develop a Crequency software, `Repository management` commands will be available. When you develop a KitX plugin, `Plugin management` commands will be available.

### Repository management

- `build` - Build the repository.
- `ci` - For CI/CD workflow.
- `clean` - Clean the repository.
- `commit` - Commit the changes to the current Git repository. This command will use Crequency commit message format.
- `fetch` - Fetch the subpackages required by the repository recursively.
- `publish` - Publish the repository to the specified directory. This command will run `fetch` and `build` before publishing. Then run publish scripts if exists.
- `test` - Run tests for the repository.

### Plugin management

- `build`, `ci`, `test` - Same as `Repository management`.
- `create` - Create a new plugin.
- `init` - Initialize the plugin by template.
- `pack` - Pack the plugin to a KXP file.

## Profile

Cheese will use the profile file to manage the repository or plugin. The profile file is a JSON file in the root directory of the repository. The Crequency software profile file is named `cheese-repo.json`, and the KitX plugin profile file is named `plugin.json`.

- About Crequency software profile, see [docs/cheese-repo.md](docs/cheese-repo.md).
- About KitX plugin profile, see [docs/plugin.md](docs/plugin.md).

## License

Cheese is licensed under the [AGPL-3.0 license](LICENSE).

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for details.

## Code of Conduct

See [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) for details.

## Contributors

[![Contributors](https://contrib.rocks/image?repo=Crequency/Cheese)](https://github.com/Crequency/Cheese/graphs/contributors)
