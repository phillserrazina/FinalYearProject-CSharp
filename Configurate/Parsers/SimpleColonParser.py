import sys

fileName = sys.argv[1]

answer = {}

with open(fileName, 'r') as file:
    lines = file.readlines()
    for line in lines:
        stringSplit = line.split('=')
        key = stringSplit[0]
        value = stringSplit[1].replace('"', '')

        answer[key] = value

print(answer)
