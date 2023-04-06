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
