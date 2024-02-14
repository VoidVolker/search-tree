# Instant search tree

Table/List fast/instant search tree for searching as fast as possible and dramatic memory usage.

# Speed

Almost instant - depends only from chars count in string. Items count doesn't affect to search speed.

# When to use

Need instant search, got slow CPU and huge RAM and big data set.

# Memory consumption

Strings items count / array data used memory / tree used memory

5 chars and 0-255 range:
```
1000000 / 55.9MB / 13.2GB
2000000 / 111.6MB / 25.2GB
```

10 chars and 0-255 range:
```
100000 / 8.1MB / 4.1GB
200000 / 16.1MB / 8GB
300000 / 24.2MB / 11.9GB
400000 / 32.2MB / 15.7GB
500000 / 40.3MB / 19.5GB
```

15 chars and 0-255 range:
```
100000 / 10.5MB / 6.6GB
200000 / 21MB / 13.1GB
300000 / 31.5MB / 19.5GB
400000 / 42MB / 25.8GB
```

20 chars and 0-255 range:
```
100000 / 13MB / 9.1GB
200000 / 25.9MB / 18.1GB
300000 / 38.9MB / 27GB
```

ArrayTree tree max size for 5 depth and 0-255 range: 8.2TB
ArrayTree nodes count for 5 depth and 0-255 range: 4 311 810 305

# Development

Code isn't perfect, old and just experiment for fun.
