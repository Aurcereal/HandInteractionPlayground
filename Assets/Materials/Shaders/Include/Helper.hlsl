void PosterizeTime_float(float Time, float Framerate, out float OutTime)
{
    OutTime = floor(Time*Framerate)/Framerate;
}

void PosterizeColor_float(float3 InColor, float Steps, out float3 OutColor) {
    float3 col = InColor;
    col = sqrt(col);
    col = floor(col*Steps)/Steps;
    col *= col;
    OutColor = col;
}

void ChromaticAbberation_float(float2 UV, float Rotation, float Spread, out float2 UV1, out float2 UV2) {
    float2 v = float2(cos(Rotation), sin(Rotation));
    UV1 = UV+v*Spread;
    UV2 = UV-v*Spread;
}

void Sample3DChecker_float(float3 Position, float Size, out float Value) {
    float3 id = floor(Position/Size);
    Value = step(abs(fmod(id.x+id.y+id.z+1000., 2.)-1.), .5);
}