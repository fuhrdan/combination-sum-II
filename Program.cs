//*****************************************************************************
//* 40. Combination Sum II  leetcode                                         **
//*****************************************************************************
//*****************************************************************************


/**
 * Return an array of arrays of size *returnSize.
 * The sizes of the arrays are returned as *returnColumnSizes array.
 * Note: Both returned array and *columnSizes array must be malloced, assume caller calls free().
 */

#define TABLE_SIZE 10000

typedef struct CombinationListNode {
    int* data;
    int size;
    struct CombinationListNode* next;
} CombinationListNode;

typedef struct HashTable {
    CombinationListNode** table;
} HashTable;

unsigned int hash(int* arr, int size) {
    unsigned int hash = 0;
    for (int i = 0; i < size; i++) {
        hash = hash * 31 + arr[i];
    }
    return hash % TABLE_SIZE;
}

int cmpfunc(const void* a, const void* b) {
    return (*(int*)a - *(int*)b);
}

int areEqual(int* arr1, int* arr2, int size) {
    for (int i = 0; i < size; i++) {
        if (arr1[i] != arr2[i]) {
            return 0;
        }
    }
    return 1;
}

void insert(HashTable* ht, int* arr, int size) {
    unsigned int idx = hash(arr, size);
    CombinationListNode* newNode = (CombinationListNode*)malloc(sizeof(CombinationListNode));
    newNode->data = (int*)malloc(size * sizeof(int));
    memcpy(newNode->data, arr, size * sizeof(int));
    newNode->size = size;
    newNode->next = ht->table[idx];
    ht->table[idx] = newNode;
}

int contains(HashTable* ht, int* arr, int size) {
    unsigned int idx = hash(arr, size);
    CombinationListNode* curr = ht->table[idx];
    while (curr) {
        if (curr->size == size && areEqual(curr->data, arr, size)) {
            return 1;
        }
        curr = curr->next;
    }
    return 0;
}

void backtrack(int* candidates, int candidatesSize, int target, int* temp, int tempSize, int start, HashTable* ht, int** result, int* returnSize, int** returnColumnSizes) {
    if (target == 0) {
        if (!contains(ht, temp, tempSize)) {
            result[*returnSize] = (int*)malloc(sizeof(int) * tempSize);
            memcpy(result[*returnSize], temp, sizeof(int) * tempSize);
            (*returnColumnSizes)[*returnSize] = tempSize;
            insert(ht, temp, tempSize);
            (*returnSize)++;
        }
        return;
    }

    for (int i = start; i < candidatesSize; i++) {
        if (i > start && candidates[i] == candidates[i - 1]) {
            continue; // skip duplicates
        }
        if (candidates[i] > target) {
            break; // no point in continuing
        }
        temp[tempSize] = candidates[i];
        backtrack(candidates, candidatesSize, target - candidates[i], temp, tempSize + 1, i + 1, ht, result, returnSize, returnColumnSizes);
    }
}

int** combinationSum2(int* candidates, int candidatesSize, int target, int* returnSize, int** returnColumnSizes) {
    *returnSize = 0;
    *returnColumnSizes = (int*)malloc(sizeof(int) * 100); // assuming max 100 combinations
    int** result = (int**)malloc(sizeof(int*) * 100); // assuming max 100 combinations
    int* temp = (int*)malloc(sizeof(int) * candidatesSize);

    qsort(candidates, candidatesSize, sizeof(int), cmpfunc);

    HashTable ht = {0};
    ht.table = (CombinationListNode**)calloc(TABLE_SIZE, sizeof(CombinationListNode*));

    backtrack(candidates, candidatesSize, target, temp, 0, 0, &ht, result, returnSize, returnColumnSizes);

    free(temp);
    for (int i = 0; i < TABLE_SIZE; i++) {
        CombinationListNode* curr = ht.table[i];
        while (curr) {
            CombinationListNode* temp = curr;
            curr = curr->next;
            free(temp->data);
            free(temp);
        }
    }
    free(ht.table);
    return result;
}
