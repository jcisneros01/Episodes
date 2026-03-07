export interface AuthTokens {
  tokenType: string
  accessToken: string
  expiresIn: number
  refreshToken: string
}

export interface CurrentUserResponse {
  id: number
  email: string
  email_confirmed: boolean
  created_at: string
}

export interface AuthUser {
  id: number
  email: string
  emailConfirmed: boolean
  createdAt: string
}
