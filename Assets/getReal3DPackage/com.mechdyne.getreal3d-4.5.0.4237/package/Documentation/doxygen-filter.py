import sys
import re

regex = re.compile(r'.*\[Tooltip\s*\(\s*"(.*)\s*"\s*\).*')

with open(sys.argv[1]) as f:
    for line in f.readlines():
        m = re.match(regex, line)
        if m:
            print(m[0] + ' /// ' + m[1])
        else:
            print(line, end='')
