# Setup
* Create `~/.config/ticket.yml` with the following contents:

```
hosts:
  - hostname: gitlab.com
    accessToken: glpat-xxxxxx
  - hostname: gitlab.example.com
    accessToken: yyyyyy
  - hostname: gitlab.example.org
    accessToken: zzzzzz 
```

* Run `ticket init` or configure `.gitconfig` as follows:

```
[ticket]
	userid = 5678
	projectid = 123456
```
