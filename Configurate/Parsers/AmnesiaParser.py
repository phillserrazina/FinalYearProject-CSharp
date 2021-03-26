import sys

fileName = sys.argv[1]

allElements = []

with open(fileName, 'r') as file:
    lines = file.readlines()
    for line in lines:
        line = line[1:-4]
        lineElements = line.split(' ')
        lineElements.pop(0)
        
        allElements.extend(lineElements)

answer = {}

for element in allElements:
    stringSplit = element.split('=')
    key = stringSplit[0]
    value = stringSplit[1].replace('"', '')
    
    answer[key] = value

print(answer)
