# Impersonator

Execute code as another user. For more info check out these old blogposts about [Impersonator](http://blog.shynet.nl/post/2008/08/08/Impersonator.aspx) and [Accessing network shares with ASP.NET](http://blog.shynet.nl/post/2008/08/05/Accessing-network-shares-with-ASPNET.aspx). 

[![Build status](http://img.shields.io/appveyor/ci/mwijnands/impersonator.svg?style=flat)](https://ci.appveyor.com/project/mwijnands/impersonator) [![NuGet version](http://img.shields.io/nuget/v/XperiCode.Impersonator.svg?style=flat)](https://www.nuget.org/packages/XperiCode.Impersonator)

## Installation

The `Impersonator` package is available at [NuGet](https://www.nuget.org/packages/XperiCode.Impersonator). To install `Impersonator`, run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console):

> #### Install-Package XperiCode.Impersonator

## Usage

```c#
using (new Impersonator(@"domainname\username", "password"))
{
	// this code is executed as the user with supplied credentials.
	var files = Directory.GetFiles(@"\\server\share\folder");
}
```