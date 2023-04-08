# Setup
* Create `~/.config/ticket.yml` with the following contents:

```
hosts:
  - hostname: gitlab.com
    access_token: glpat-xxxxxx
  - hostname: gitlab.example.com
    access_token: yyyyyy
  - hostname: gitlab.example.org
    access_token: zzzzzz 
```

* Run `ticket init` or configure `.gitconfig` as follows:

```
[ticket]
	userid = 5678
	projectid = 123456
```

# Usage
```shell
# Initialize repository for Ticket
$ ticket init

# Create new issues
$ ticket create
> call saul
# Create issue with description
> call saul: get phone number from jesse
# Create issue with assignee
> call saul @jpinkman
# Create issue with labels
> call saul #feature #backlog

# Assign issue to yourself
$ ticket assign 456 me
# Assign issue to yourself (shorthand)
$ ticket assign 456
# Assign issue to user
$ ticket assign 456 swhite

# Remove assignees from issue
$ ticket unassign 456

# Close issue
$ ticket close 456

# List issues assigned to you
$ ticket list 456
# List issues
$ ticket list 456 -a
```
