window.downloadFileFromStream = async (fileName, stream, mediaType) => {
    const arrayBuffer = await stream.arrayBuffer();
    const blob = new Blob([arrayBuffer], { type: mediaType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    a.click();
    URL.revokeObjectURL(url);
};
