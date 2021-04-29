import sys

fileName = sys.argv[1]

answer = {}
comparator = "="

with open(fileName, 'r') as file:
    lines = file.readlines()
    for line in lines:
        if comparator not in line:
            continue
        
        stringSplit = line.split(comparator)
        key = stringSplit[0]
        value = stringSplit[1].replace('"', '')

        answer[key] = value

print(answer)
