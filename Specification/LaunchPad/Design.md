# About

`LaunchPad` is designed to manage your development environment

For example:

When you powered on your computer, you will need to open a lot of applications, such as `IDE`, `Terminal`, `Browser`, etc.
With this module, you can open all of them with one command.

## Profile

`LaunchPad` supports multiple profiles.

A profile config file looks like this:

```yaml
name: StartUp Profile
define:
  - operation:
    name: close_window
    value:
      - win32:
          - send_messages:
              - close_window:
root: # root project (all the projects will construct a tree)
  - project:
    name: Clash Verge # name
    exec: path/to/clash-verge/executable # path to the executable
    sub_projects: # sub-projects (optional), sub_projects will
      - project:
        name: Visual Studio Code Editors
        exec: code
        base: $HOME/Projects # base path to run the exec
        args: # arguments (joined with space)
          - --new-window
        parallel_options: # parallel options (optional), this will be used to expand the task template
          variables: # variables (optional)
            - project: ["project1", "project2"]
          delay: 3 # each parallel task will wait for 3 seconds after the previous one
        parallel: # parallel task or task template
          - task:
            paralleled_task: true # control whether use `parallel_options` to expand this task template
            args: # appended argument
              - $_/${project}.code-workspace # $_ is the special variable refer to base path, ${} can refer parallel variables
      - project:
        name: IPFS Desktop
        exec: path/to/ipfs-desktop/executable
        after:
          - operation: close_window # you can call operation directly by name
  - project:
    name: Kleopatra
    exec: path/to/kleopatra/executable
    depends_on:
      - project: "Clash Verge" # you can also control dependency tree by depends_on attribute
    after:
      - operation: # operation is a special task, it will be executed after the project finished
          - win32:
              - send_messages:
                  - close_window:
```

