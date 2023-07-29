using System.Collections.Generic;

public sealed class ArrayX
{
    /**
     * Converts generic ArraList to Array
     *
     * @param componentType    Class type, i.e. if List<String> is converted
     *                         then componentType = String.class
     * @param List        List of <T> to be converted to T[]
     *
     * @return                 Array T[] including elements of List<T>
     */
    public static T[] toArray<T>(List<T> list)
    {
        return list.ToArray();
    }
    /**
     * Converts generic ArraList to Array
     *
     * @param componentType    Class type, i.e. if List<String> is converted
     *                         then componentType = String.class
     * @param stack            Stack of <T> to be converted to T[]
     *
     * @return                 Array T[] including elements of Stack<T>
     */
    public static T[] toArray<T>(Stack<T> stack)
    {
        return stack.ToArray();
    }
}