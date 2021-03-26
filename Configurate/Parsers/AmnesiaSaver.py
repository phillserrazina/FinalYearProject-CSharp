import sys
import json

path = sys.argv[1]
data = sys.argv[2]

contents = json.loads(data[1:-1])

def Add(varName):
    answer = varName
    answer += '="'
    answer += str(contents[varName])
    answer += '" '
    return answer

dump = "<Main "
dump += Add("ShowMenu")
dump += Add("SaveConfig")
dump += Add("DefaultProfileName")
dump += Add("UpdateLogActive")
dump += Add("LoadDebugMenu")
dump += Add("StartLanguage")
dump += Add("ScreenShotExt")
dump += Add("ForceCacheLoadingAndSkipSaving")
dump += Add("ShowPreMenu")
dump += "/>\n"

dump += "<Graphics "
dump += Add("TextureQuality")
dump += Add("TextureFilter")
dump += Add("TextureAnisotropy")
dump += Add("Gamma")
dump += Add("Shadows")
dump += Add("SSAOActive")
dump += Add("SSAOSamples")
dump += Add("GBufferType")
dump += Add("NumOfGBufferTextures")
dump += Add("OcclusionTestLights")
dump += Add("SSAOResolution")
dump += Add("WorldReflection")
dump += Add("Refraction")
dump += Add("ShadowsActive")
dump += Add("ShadowQuality")
dump += Add("ShadowResolution")
dump += Add("ParallaxQuality")
dump += Add("ParallaxEnabled")
dump += Add("EdgeSmooth")
dump += Add("ForceShaderModel3And4Off")
dump += Add("PostEffectBloom")
dump += Add("PostEffectImageTrail")
dump += Add("PostEffectSepia")
dump += Add("PostEffectRadialBlur")
dump += Add("PostEffectInsanity")
dump += "/>\n"

dump += "<Engine "
dump += Add("LimitFPS")
dump += Add("SleepWhenOutOfFocus")
dump += "/>\n"

dump += "<Screen "
dump += Add("Width")
dump += Add("Height")
dump += Add("Display")
dump += Add("FullScreen")
dump += Add("Vsync")
dump += "/>\n"

dump += "<Physics "
dump += Add("PhysicsAccuracy")
dump += Add("UpdatesPerSec")
dump += "/>\n"

dump += "<MapLoad "
dump += Add("FastPhysicsLoad")
dump += Add("FastStaticLoad")
dump += Add("FastEntityLoad")
dump += "/>\n"

dump += "<Sound "
dump += Add("Device")
dump += Add("Volume")
dump += Add("MaxChannels")
dump += Add("StreamBuffers")
dump += Add("StreamBufferSize")
dump += "/>\n"

with open(path, 'w') as output:
    try:
        output.write(dump)
        print("Saved Successfully.")

    except Exception as e:
        print("Error:", "Invalid JSON format (", e, ")")
