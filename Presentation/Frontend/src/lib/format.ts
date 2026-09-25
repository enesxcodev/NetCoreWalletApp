const currency = new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY', minimumFractionDigits: 2, maximumFractionDigits: 2 })
const dateTime = new Intl.DateTimeFormat('tr-TR', { dateStyle: 'medium', timeStyle: 'short' })

/** API'den gelen tutarı Türk lirası biçiminde gösterir; hesaplama yapmaz. */
export function formatMoney(value: number) { return currency.format(value) }

/** ISO tarihini kullanıcının yerel saatine göre okunur hale getirir. */
export function formatDate(value: string) { return dateTime.format(new Date(value)) }
