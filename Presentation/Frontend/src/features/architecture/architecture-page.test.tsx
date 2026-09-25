import { fireEvent, render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { ArchitecturePage } from './architecture-page'

describe('mimari akış ekranı', () => {
  it('Outbox yolunu ve broker kesintisinde bekleyen olayı açıklar', () => {
    render(<ArchitecturePage />)
    expect(screen.getByText(/MSSQL → Outbox → RabbitMQ → MongoDB → Redis/)).toBeInTheDocument()
    expect(screen.getByText(/Bu ekran canlı sağlık kontrolü değildir/)).toBeInTheDocument()
    fireEvent.click(screen.getByRole('button', { name: /RabbitMQ kesintisini simüle et/ }))
    expect(screen.getByText(/Olay Outbox’ta bekliyor/)).toBeInTheDocument()
    fireEvent.click(screen.getByRole('button', { name: /Bağlantıyı geri getir/ }))
    expect(screen.getByText(/Redis geçmiş sürümü artırıldı/)).toBeInTheDocument()
  })
})
