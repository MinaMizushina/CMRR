# What's this?
This tool monitors Windows clipboard, and replace with using Regular Expression.
It can have multiple settings, and work with all of them.

So, this tool only replaces clipboard setting, on conditions both of below:
- Clipboard updated with any software
- With regular expression setting as test pattern

And, only works with clipboard TEXT, not HTML or extended formats.

# How to use?
Build and write setting.xml file.
By laziness of me, this tool do not have something like "Setting" window. This means only updating "setting.xml" by editor can change behavior.

``` setting.xml
<?xml version="1.0" encoding="utf-8" ?>
<CMRRSetting>
	<setting
		name="Test Setting"
		match="test message"
		replaceTo="app running"
		notifySeconds ="5"
		goNext="true" />
</CMRRSetting>
```
This is most simple pattern to make this tool working.
Each `setting` element works as replacement setting.

Be sure that, regular expression match tries in XML elements order.
When multiple setting matches to clipboard text, basically first one will be applied.
(And, if `goNext` enabled, next one will be applied. This means, if replaced text with first setting matches second pattern, second pattern works and the text will be replaced, like chain working.)

Each attributes works below:

|Attribute|Description|
|:--------|:---------|
|name|Setting name. This only used for |
|match|Test pattern to check clipboard value. When this value matches clipboard text, this app replaces it with `replaceTo` attr value.|
|replaceTo|Regular Expression as using match.|
|notifySeconds|Set integer value. If this value upper 0, application shows balloon text when clipboard text replaced, and it disappears  with seconds on set value.|
|goNExt|set `true` or `false`. When set `true`, testing cotinues even `match` pattern mached and clipboard text replaced. In case this attribute set as `false`, execution stops with replacing text in match. `setting` elements above will ignored this time.|

Setting files are read when app starts.
When you updated setting file, you need to terminate app and re-start.

Be sure that you need to escape some characters of regular expression, to set as XML attribute.
For example `<` should be `&lt;`.
Maybe  you'd better to use XML enabled editor, like Visual Studio Code :)

# License
MIT License (Also known as X11 License) is applied for this application / source.
