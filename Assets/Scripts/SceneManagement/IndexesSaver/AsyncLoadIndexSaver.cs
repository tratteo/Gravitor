public static class AsyncLoadIndexSaver
{
    private static int indexToPreload;
    public static int GetSceneIndexToPreload() { return indexToPreload; }
    public static void SetIndexToPreload(int index) { indexToPreload = index; }
}
