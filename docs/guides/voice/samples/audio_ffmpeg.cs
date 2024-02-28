await using var output = ffmpeg.StandardOutput.BaseStream;
await using var kook = _audioClient.CreatePcmStream(AudioApplication.Voice);
try
{
    await output.CopyToAsync(kook, cancellationToken);
}
finally
{
    await kook.FlushAsync(cancellationToken);
}
