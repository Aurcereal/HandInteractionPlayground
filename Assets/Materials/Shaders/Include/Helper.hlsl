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

void Sample3DStripe_float(float3 Position, float RepeatSize, float StripeFactor, out float Value) {
    float along = dot(Position, float3(1.,1.,1.)/sqrt(3.));
    float lalong = along-floor(along/RepeatSize)*RepeatSize;
    lalong = abs(lalong-RepeatSize*.5);
    Value = step(lalong, RepeatSize*.5*StripeFactor);
}

void Sample3DCubeRepeat_float(float3 Position, float RepeatSize, float BoxSizeFactor, out float Value) {
    float3 id = floor(Position/RepeatSize)*RepeatSize;
    float3 lp = Position-id-RepeatSize*.5;
    lp = abs(lp);
    float cr = max(lp.x, max(lp.y, lp.z));
    
    float currBoxSize = RepeatSize*.5*BoxSizeFactor;
    Value = step(cr, currBoxSize);
}

void Sample3DDotRepeat_float(float3 Position, float RepeatSize, float DotSizeFactor, out float Value) {
    float3 id = floor(Position/RepeatSize)*RepeatSize;
    float3 lp = Position-id-RepeatSize*.5;
    float r = length(lp);
    
    float currDotSize = RepeatSize*.5*DotSizeFactor;
    Value = step(r, currDotSize);
}

void Sample3DSpherePattern_float(float3 Position, float RepeatSize, float DotSizeFactor, float Thickness, out float Value) {
    float3 id = floor(Position/RepeatSize)*RepeatSize;
    float3 lp = (Position-id-RepeatSize*.5)/(RepeatSize*.5); // *.5 is an error but looks cool?

    float exists = 0.;
    for(int i=0; i<2; i++) {
        exists = max(exists,
            step(abs(length(lp)-DotSizeFactor), Thickness)
        );
        lp += .5;
        lp = fmod(lp, .5)/.5;
        lp -= .5;
    }
    
    float smallDot;
    Sample3DDotRepeat_float(Position, RepeatSize, DotSizeFactor*.2, smallDot);

    Value = max(exists, smallDot);
}

void BorderFadeout_float(float2 UV, float2 Range, out float Falloff) {
    float2 p = UV-.5;
    p = abs(p);
    float dist = min(.5-p.x, .5-p.y);
    Falloff = smoothstep(Range.x, Range.y, dist);
}