[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=U7ZWQ2WHFEWH4)

# Common Utils for Unity
A collection of essential tools for any Unity project.

## Install ##

**Installation must be performed by project.**

1. Open the Package Manager in Unity (menu Window / Package Manager).
2. Press the "+" button at the top left corner of the Package Manager panel and select "Add package from git URL..."
3. When promted, enter the URL https://github.com/edcasillas/unity-common-utils.git

Alternatively, you can manually add the following line to your Packages/manifest.json file under dependencies:

    "com.ecasillas.commonutils": "https://github.com/edcasillas/unity-common-utils.git"

Open Unity again; the Package Manager will run and the package will be installed.

## Update ##

1. Open the Package Manager in Unity (menu Window / Package Manager).
2. Look for the "Common Utils for Unity" package in the list pf installed packages and select it.
3. Press the "Update" button.

Alternatively, you can manually remove the version lock the Package Manager creates in Packages/manifest.json so when it runs again it gets the newest version. The lock looks like this:

```
    "com.ecasillas.commonutils": {
      "hash": "someValue",
      "revision": "HEAD"
    }
```
## Features ##
Please refer to the [wiki](https://github.com/edcasillas/unity-common-utils/wiki) for a full description on the features and their documentation.
