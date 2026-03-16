using System;

while (true)
{
    Console.Write("Введите первую строку: ");
    string s1 = Console.ReadLine();

    if (s1 == "exit")
        break;

    Console.Write("Введите вторую строку: ");
    string s2 = Console.ReadLine();

    // переводим строки в верхний регистр
    s1 = s1.ToUpper();
    s2 = s2.ToUpper();

    int distance = Distance(s1, s2);

    Console.WriteLine($"Расстояние Дамерау-Левенштейна: {distance}");
    Console.WriteLine();
}

static int Distance(string s1, string s2)
{
    int m = s1.Length;
    int n = s2.Length;

    int[,] d = new int[m + 1, n + 1];

    for (int i = 0; i <= m; i++)
        d[i, 0] = i;

    for (int j = 0; j <= n; j++)
        d[0, j] = j;

    for (int i = 1; i <= m; i++)
    {
        for (int j = 1; j <= n; j++)
        {
            int cost;

            if (s1[i - 1] == s2[j - 1])
                cost = 0;
            else
                cost = 1;

            int delete = d[i - 1, j] + 1;
            int insert = d[i, j - 1] + 1;
            int replace = d[i - 1, j - 1] + cost;

            int min = Math.Min(delete, Math.Min(insert, replace));

            if (i > 1 && j > 1 &&
                s1[i - 1] == s2[j - 2] &&
                s1[i - 2] == s2[j - 1])
            {
                min = Math.Min(min, d[i - 2, j - 2] + 1);
            }

            d[i, j] = min;
        }
    }

    return d[m, n];
}