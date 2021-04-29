import sys
import json

path = sys.argv[1]
data = sys.argv[2]

contents = json.loads(data[1:-1])
dump = ""

for key in contents:
    dump += str(key)
    dump += "="
    dump += str(contents[key])
    dump += "\n"

with open(path, 'w') as output:
    try:
        output.write(dump)
        print("Saved Successfully.")

    except Exception as e:
        print("Error:", "Invalid JSON format (", e, ")")
