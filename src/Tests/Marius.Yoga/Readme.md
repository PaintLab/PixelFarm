This is a managed port of [Yoga layout engine](https://github.com/facebook/yoga) without any native dependencies.

I recommend using oficial yoga library if native libraries are not an issue.

This port is not api compatible with csharp bindings of the original yoga. Undefined float values are expressed as (nullable float) null. Original library uses float.NaN. Printing is missing from this port (thus the tests have been commented out).


---

see original => https://github.com/marius-klimantavicius/yoga