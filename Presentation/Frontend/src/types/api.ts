/** Backend'in standart cevap zarfını tipli olarak taşır. */
export interface ApiResult<T> {
  isSuccess: boolean
  data?: T | null
  error?: string | null
  errors?: string[] | null
}

/** API hatasını arayüzün gösterebileceği tek bir mesaja dönüştürür. */
export class ApiError extends Error {
  constructor(
    message: string,
    public readonly status: number,
  ) {
    super(message)
    this.name = 'ApiError'
  }
}
