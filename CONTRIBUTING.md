# Guideline for a healthy repo

1. [`README.md`](#readme-md)
1. [CI/CD](#ci-cd)
1. [Commits](#commits)
1. [Issues](#issues)
1. [Contributors](#contributors)
1. [Releases](#releases)
1. [Tooling](#tooling)
1. [Open source projects](#open-source-projects)




## `README.md`

All Git repositories must have a `README.md`. This file should be written 
in Markdown and as a minimum contain a title and a description of the
repository.

Other topics commonly included in repository READMEs: how to install
and run, link to documentation, status badges, code samples, citations. For
information about READMEs on GitHub, see 
[GitHub About READMEs](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/customizing-your-repository/about-readmes).


## CI/CD

Is there a defined CI/CD pipeline?

With a CI/CD pipeline, we here mean that every patch-set (commit) is
tested automatically by a server running in the cloud, preferably using Docker.

Is the build and test status reported back to GitHub in a way that
enables GitHub to put a green checkmark or red cross next to the commit
that is being built?


## Commits

How do the commit looks?  How do the commit messages look?

A commit should always be the least amount of change that fixes a task
(implements a feature, fixes a bug) or possibly improves the code
quality.

Every commit that modifies the code base's behavior should contain
tests.

The _commit message_ should be in the format `50+72*` and the first line
should be written in imperative mode.  See the [How to Write a Git
Commit Message](https://chris.beams.io/posts/git-commit/) for some tips.

It is beneficial for a code base that at each commit, the code base
builds and all tests pass.  It is therefore paramount with a properly
configured CI/CD that is being run prior to merging to trunk.


## Issues

If the repository contains functional code, is there an issue handling system?
(Github-issues, jira, trello, azure devops-board or similar?) If the issue
handling is not in _Github-issues_ (A good choice as it keeps issues close to
the repo), `README.md` should link to where the issue-handling is managed.

Transparent processes are good. Keep the issue list and your _Scrum-_ or _Kanban
board_ just as accessible and open for your colleagues as your repository.



## Contributors

How many have contributed the last month?  How are contributions made?

A bad sign is a single programmer committing directly to master.

A good sign is several programmers committing via pull requests that are
reviewed and approved by the others.

It is easy to see in GitHub whether commits typically come from pull requests (PRs) or whether they are pushed directly to master.  Are PRs used extensively?  Are all PRs reviewed with passing tests before merge?

In GitHub there is a [_Contributor graph_](https://help.github.com/en/github/visualizing-repository-data-with-graphs/viewing-a-projects-contributors) that can be used to visualize this too.


## Releases

Are there releases in the repo?  Are these named using
[_Semver_](https://semver.org/)?

Using Semver is important in library and API projects since it
communicates to users of your library what they can expect of behavior
change over time.

Semver is not equally important in end-user applications, but we still
recommend using at least [_Calver_](https://calver.org/) (e.g. 2016.04,
2018.10, etc.).

## Tooling

For libraries and APIs, are there code samples in the `README.md` file?

Does the project use standard build automation and install tooling such as `npm`, `ant`/`maven`, `setuptools`/`pip`, `cmake` etc?

Are the tests built on standard frameworks as well, e.g. `junit`, `pytest` etc.?

## Open source projects

### LICENSE

If the project is open source, is there a `LICENSE.md` license file?

The _Unlicense_ is not valid.  See our internal [Open Source Licensing
guidelines](https://github.com/equinor/it-professional-network/blob/master/doc/open_source/licenses.md)
for how to choose licenses.

### Contributing

Open source projects should always include contributing guidelines. Add a
`CONTRIBUTING.md` file to the root of your repo, or include a _Contributing_
paragraph in your `README.md`.

Depending on your needs, a valuable contribution could appear in many forms.
From a bug report, code or perhaps as improved documentation.

### How can users reach you with a suggestion for a change?

You might already use GitHub issues, or perhaps you prefer another way of
communication? Either way, make it clear and easy to contact you and discuss
suggestions for changes. Facilitation of a good discussion on forehand usually
results in better contributions.

### The pull requests

Give a brief explanation of your workflow, and your expectations for what
should be included in a PR. From styleguides, commit messages, to tests, and
other standards you set for your project.

Clear out any process details from the start, so that you and your contributors
can focus on the actual changes that matters right away.
