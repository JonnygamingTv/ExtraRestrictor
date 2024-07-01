# ExtraRestrictor
Item restriction plugin loosely based off of LeeIzAZombie's ItemRestrictions, but using events instead of constant checks for no lag. Also has group based restrictions.

~~Documentation available here: https://iceplugins.xyz/ExtraRestrictor~~

## Config

```xml
<?xml version="1.0" encoding="utf-8"?>
<ExtraRestrictorConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Restricted>
    <Item BypassPermission="bypass.permission">ItemID</Item>
  </Restricted>
  <IgnoreAdmins>true</IgnoreAdmins>
</ExtraRestrictorConfiguration>
```

## Scripting
```js
let bypasses=['cop','pirate','thief'];
let items=[
[40500, 40501, 40502, 40503, 40504, 40505, 40506, 40507, 40508, 40509, 40510, 40511, 40512, 40513, 40514, 40515, 40517, 40518, 40519, 40520],
[40521],
[40524, 40523, 40522]
];
let str=[];
for(let i=0;i<bypasses.length;i++){
    for(let y=0;y<items[i].length;y++){
        str[str.length]=`    <Item BypassPermission="bypass.${bypasses[i]}">${items[i][y]}</Item>`;
    }
}
console.log(str.join('\n'));
```
